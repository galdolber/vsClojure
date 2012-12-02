using System;
using System.Collections.Generic;
using System.Linq;
using Clojure.Code.Editing.PartialUpdate;
using Clojure.Workspace.TextEditor;
using Microsoft.VisualStudio.Text;

namespace Clojure.VisualStudio.Workspace.TextEditor
{
	public class VisualStudioClojureTextBuffer : IClojureTextBufferStateListener, IUserActionSource
	{
		private readonly ITextBuffer _textBuffer;
		private readonly List<IUserActionListener> _listeners;

		public VisualStudioClojureTextBuffer(ITextBuffer textBuffer)
		{
			_textBuffer = textBuffer;
			_textBuffer.Changed += OnBufferChange;
			_listeners = new List<IUserActionListener>();

			var clojureTextBuffer = new ClojureTextBuffer();
			var bracerMatchingTagger = new BraceMatchingTagger(textBuffer);
			var tokenTagger = new ClojureTokenTagger(textBuffer);
			var autoIndent = new ClojureAutoIndent(clojureTextBuffer);

			clojureTextBuffer.AddStateChangeListener(this);
			clojureTextBuffer.AddStateChangeListener(bracerMatchingTagger);
			clojureTextBuffer.AddStateChangeListener(tokenTagger);
			AddUserActionListener(clojureTextBuffer);

			textBuffer.Properties.AddProperty(bracerMatchingTagger.GetType(), bracerMatchingTagger);
			textBuffer.Properties.AddProperty(tokenTagger.GetType(), tokenTagger);
			textBuffer.Properties.AddProperty(autoIndent.GetType(), autoIndent);
			textBuffer.Properties.AddProperty(clojureTextBuffer.GetType(), clojureTextBuffer);
			textBuffer.Properties.AddProperty(GetType(), this);
		}

		public void AddUserActionListener(IUserActionListener listener)
		{
			_listeners.Add(listener);
		}

		private void OnBufferChange(object sender, TextContentChangedEventArgs e)
		{
			var changeData = e.Changes.Select(
				change => new TextChangeData(change.OldPosition, change.Delta, Math.Max(change.NewSpan.Length, change.OldSpan.Length))).ToList();

			_listeners.ForEach(l => l.Edit(changeData, _textBuffer.CurrentSnapshot.GetText()));
		}

		public void BufferChanged(string newText)
		{
			_textBuffer.Replace(new Span(0, _textBuffer.CurrentSnapshot.Length), newText);
		}

		public void TokensChanged(TextBufferSnapshot snapshot, BufferDiffGram diffGram)
		{

		}
	}
}