using System.Collections.Generic;
using Clojure.Workspace.TextEditor;
using Microsoft.VisualStudio.Text.Editor;

namespace Clojure.VisualStudio.Workspace.TextEditor
{
	public class VisualStudioClojureTextView : IUserActionSource
	{
		private readonly ITextView _currentWpfTextView;
		private readonly List<IUserActionListener> _actionListeners;

		public VisualStudioClojureTextView(ITextView view)
		{
			_currentWpfTextView = view;
			_currentWpfTextView.Caret.PositionChanged += CaretPositionChanged;
			_actionListeners = new List<IUserActionListener>();
		}

		private void CaretPositionChanged(object sender, CaretPositionChangedEventArgs e)
		{
			_actionListeners.ForEach(l => l.OnCaretPositionChange(e.NewPosition.BufferPosition.Position));
		}

		public void AddUserActionListener(IUserActionListener listener)
		{
			_actionListeners.Add(listener);
		}

		public void Format()
		{
			_actionListeners.ForEach(l => l.Format());
		}

		public void CommentSelectedLines()
		{
			int startPosition = _currentWpfTextView.Selection.Start.Position.GetContainingLine().Start.Position;
			int endPosition = _currentWpfTextView.Selection.End.Position.GetContainingLine().End.Position;
			_actionListeners.ForEach(l => l.CommentLines(startPosition, endPosition));
		}

		public void UncommentSelectedLines()
		{
			int startPosition = _currentWpfTextView.Selection.Start.Position.GetContainingLine().Start.Position;
			int endPosition = _currentWpfTextView.Selection.End.Position.GetContainingLine().End.Position;
			_actionListeners.ForEach(l => l.UncommentLines(startPosition, endPosition));
		}
	}
}