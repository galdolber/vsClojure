using System;
using System.Collections.Generic;
using System.Linq;
using Clojure.Code.Editing.Formatting;
using Clojure.Code.Editing.PartialUpdate;
using Clojure.Workspace.TextEditor;
using Microsoft.VisualStudio.Text;
using Clojure.System.Collections;

namespace Clojure.VisualStudio.Workspace.TextEditor
{
	public class VisualStudioClojureTextBuffer : IUserActionListener
	{
		private readonly ITextBuffer _textBuffer;
		private readonly List<IClojureTextBufferStateListener> _stateListeners;
		private TextBufferSnapshot _snapshot;

		public VisualStudioClojureTextBuffer(ITextBuffer textBuffer)
		{
			_textBuffer = textBuffer;
			_textBuffer.Changed += OnBufferChange;
			_snapshot = TextBufferSnapshot.Empty();
			_stateListeners = new List<IClojureTextBufferStateListener>();
			textBuffer.Properties.AddProperty(GetType(), this);
		}

		public ITextSnapshot GetTextSnapshot()
		{
			return _textBuffer.CurrentSnapshot;
		}

		public TextBufferSnapshot GetTokenSnapshot()
		{
			return _snapshot;
		}

		public void AddStateChangeListener(IClojureTextBufferStateListener listener)
		{
			_stateListeners.Add(listener);
		}

		public void Format()
		{
			_textBuffer.Replace(new Span(0, _textBuffer.CurrentSnapshot.Length), new AutoFormat().Format(_snapshot.Tokens, 2));
		}

		public void CommentLines(int startIndex, int endIndex)
		{
			throw new NotImplementedException();
		}

		public void UncommentLines(int startPosition, int endPosition)
		{
			throw new NotImplementedException();
		}

		public void OnCaretPositionChange(int newPosition)
		{
			_snapshot = _snapshot.ChangeCaretPosition(newPosition);
			FireCaretChangeEvent();
		}

		private void FireCaretChangeEvent()
		{
			_stateListeners.ForEach(l => l.CaretChanged(_snapshot));
		}

		private void OnBufferChange(object sender, TextContentChangedEventArgs e)
		{
			var changes = e.Changes.Select(change => new TextChangeData(change.OldPosition, change.Delta, Math.Max(change.NewSpan.Length, change.OldSpan.Length))).ToList();
			FireTokenUpdateEvent(CreateDiffGrams(changes));
		}

		private IEnumerable<BufferDiffGram> CreateDiffGrams(IEnumerable<TextChangeData> changes)
		{
			return changes.Select(change => _snapshot.Tokens.ApplyChange(change, _textBuffer.CurrentSnapshot.GetText()));
		}

		private void FireTokenUpdateEvent(IEnumerable<BufferDiffGram> diffGrams)
		{
			diffGrams.ToList().ForEach(diffGram => _stateListeners.ForEach(l => l.TokensChanged(_snapshot, diffGram)));
		}

		public void InvalidateTokens()
		{
			FireTokenUpdateEvent(CreateDiffGrams(new TextChangeData(0, _textBuffer.CurrentSnapshot.Length).SingletonAsList()));
		}
	}
}