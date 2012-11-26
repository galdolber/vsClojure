using System.Collections.Generic;
using System.Linq;
using Clojure.Code.Editing.Commenting;
using Clojure.Code.Editing.Formatting;
using Clojure.Code.Editing.PartialUpdate;

namespace Clojure.Workspace.TextEditor
{
	public class ClojureTextBuffer : ITextEditorActionListener
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

		public void TextChanged(List<TextChangeData> changes, string textSnapshot)
		{
			var diffGrams = changes.Select(change => _snapshot.Tokens.ApplyChange(change, textSnapshot)).ToList();
			diffGrams.ForEach(diffGram => _listeners.ForEach(l => l.TokensChanged(diffGram)));
		}

		public void Format()
		{
			var result = new AutoFormat().Format(_snapshot.Tokens, 2);
			_snapshot.Tokens.Clear();
			_listeners.ForEach(l => l.BufferChanged(result));
		}

		public void CommentSelection(List<string> selectedLines)
		{
			_listeners.ForEach(l => l.ReplaceSelectedLines(new BlockComment().Execute(selectedLines)));
		}
	}
}