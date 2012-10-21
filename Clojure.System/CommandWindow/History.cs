using System.Collections.Generic;

namespace Clojure.System.CommandWindow
{
	public class History : IHistoryCommandListener, ISubmitCommandListener
	{
		private LinkedListNode<string> _currentlySelectedHistoryItem;
		private readonly LinkedList<string> _history;
		private readonly ICommandWindow _commandWindow;

		public History(ICommandWindow commandWindow)
		{
			_history = new LinkedList<string>();
			_history.AddLast("");
			_currentlySelectedHistoryItem = _history.First;
			_commandWindow = commandWindow;
		}

		public void Submit(string expression)
		{
			// If user has something selected in history and they submit it, move it to the front?
			// Don't submit empty expressions.
			// Should an empty string always be in the history?
			// Can I use an empty as the default history item selection?

			if (!string.IsNullOrEmpty(expression.Trim()))
			{
				_history.AddFirst(expression);
			}
			if (_currentlySelectedHistoryItem.Value == expression)
			{
				_history.Remove(_currentlySelectedHistoryItem);
				_history.AddFirst(_currentlySelectedHistoryItem.Value);
				_currentlySelectedHistoryItem = _history.First;
			}
		}

		public void Next()
		{
			if (_currentlySelectedHistoryItem.Next != null)
			{
				_currentlySelectedHistoryItem = _currentlySelectedHistoryItem.Next;
			}

			_commandWindow.ReplaceCurrentExpressionWith(_currentlySelectedHistoryItem.Value);
		}

		public void Previous()
		{
			if (_currentlySelectedHistoryItem.Previous != null)
			{
				_currentlySelectedHistoryItem = _currentlySelectedHistoryItem.Previous;
			}

			_commandWindow.ReplaceCurrentExpressionWith(_currentlySelectedHistoryItem.Value);
		}
	}
}
