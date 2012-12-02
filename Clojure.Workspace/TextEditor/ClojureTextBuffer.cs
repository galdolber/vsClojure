using System;
using System.Collections.Generic;
using System.Linq;
using Clojure.Code.Editing.Commenting;
using Clojure.Code.Editing.Formatting;
using Clojure.Code.Editing.PartialUpdate;
using Clojure.Code.Parsing;

namespace Clojure.Workspace.TextEditor
{
	public class ClojureTextBuffer : IUserActionListener
	{
		private TextEditorSnapshot _snapshot;
		private readonly List<IClojureTextBufferStateListener> _listeners;

		public ClojureTextBuffer()
		{
			_snapshot = TextEditorSnapshot.Empty;
			_listeners = new List<IClojureTextBufferStateListener>();
		}

		public void AddStateChangeListener(IClojureTextBufferStateListener listener)
		{
			_listeners.Add(listener);
		}

		public TextEditorSnapshot GetSnapshot()
		{
			return _snapshot;
		}

		public void Edit(List<TextChangeData> changes, string textSnapshot)
		{
			var diffGrams = changes.Select(change => _snapshot.Tokens.ApplyChange(change, textSnapshot)).ToList();
			diffGrams.ForEach(diffGram => _listeners.ForEach(l => l.TokensChanged(_snapshot, diffGram)));
		}

		public void Format()
		{
			var result = new AutoFormat().Format(_snapshot.Tokens, 2);
			_snapshot.Tokens.Clear();
			_listeners.ForEach(l => l.BufferChanged(result));
		}

		public void CommentLines(int startIndex, int endIndex)
		{
			throw new NotImplementedException();
		}

		public void UncommentLines(int startPosition, int endPosition)
		{
			throw new NotImplementedException();
		}
	}
}