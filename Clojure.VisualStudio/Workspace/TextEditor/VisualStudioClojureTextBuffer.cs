using System;
using System.Collections.Generic;
using System.Linq;
using Clojure.Code.Editing.PartialUpdate;
using Clojure.Code.Parsing;
using Clojure.VisualStudio.Editor.BraceMatching;
using Clojure.VisualStudio.Workspace.TextEditor.SmartIndent;
using Clojure.VisualStudio.Workspace.TextEditor.Tagging;
using Clojure.Workspace.TextEditor;
using Microsoft.VisualStudio.Text;

namespace Clojure.VisualStudio.Workspace.TextEditor
{
	public class VisualStudioClojureTextBuffer : IClojureTextBufferStateListener, IUserActionSource
	{
		private readonly ITextBuffer _textBuffer;
		private readonly List<IUserActionListener> _listeners;

		public VisualStudioClojureTextBuffer(ITextBuffer textBuffer, ClojureTextBuffer clojureTextBuffer)
		{
			_textBuffer = textBuffer;
			_textBuffer.Changed += OnBufferChange;
			_listeners = new List<IUserActionListener>();

			var bracerMatchingTagger = textBuffer.Properties.GetOrCreateSingletonProperty(() => new BraceMatchingTagger(textBuffer));
			var tokenTagger = textBuffer.Properties.GetOrCreateSingletonProperty(() => new ClojureTokenTagger(textBuffer));
			var autoIndent = textBuffer.Properties.GetOrCreateSingletonProperty(() => new ClojureAutoIndent());
			clojureTextBuffer.AddStateChangeListener(this);
			clojureTextBuffer.AddStateChangeListener(bracerMatchingTagger);
			clojureTextBuffer.AddStateChangeListener(tokenTagger);
			clojureTextBuffer.AddStateChangeListener(autoIndent);
			AddUserActionListener(clojureTextBuffer);
		}

		public void OnBufferChange(object sender, TextContentChangedEventArgs e)
		{
			var changeData = e.Changes.Select(
				change => new TextChangeData(change.OldPosition, change.Delta, Math.Max(change.NewSpan.Length, change.OldSpan.Length))).ToList();

			_listeners.ForEach(l => l.Edit(changeData, _textBuffer.CurrentSnapshot.GetText()));
		}

		public void BufferChanged(string newText)
		{
			_textBuffer.Replace(new Span(0, _textBuffer.CurrentSnapshot.Length), newText);
		}

		public void TokensChanged(TextEditorSnapshot snapshot, BufferDiffGram diffGram)
		{
		}

		public void AddUserActionListener(IUserActionListener listener)
		{
			_listeners.Add(listener);
		}
	}
}