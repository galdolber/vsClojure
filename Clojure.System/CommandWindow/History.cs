using System.Collections.Generic;

namespace Clojure.Base.CommandWindow
{
	public class History : IHistoryCommandListener, ISubmitCommandListener
	{
		private LinkedListNode<string> _currentlySelectedHistoryItem;
		private readonly LinkedList<string> _history;
		private readonly IHistoryEventListener _historyEventListener;

		public History(IHistoryEventListener historyEventListener)
		{
			_history = new LinkedList<string>();
			_historyEventListener = historyEventListener;
		}

		public void Submit(string expression)
		{
            if (!IsFirstInHistory(expression) && !string.IsNullOrEmpty(expression.Trim()))
			{
                _history.AddFirst(expression);
			}

			_currentlySelectedHistoryItem = null;
		}

        private bool IsFirstInHistory(string expression)
        {
            return _history.Count > 0 && _history.First.Value == expression;
        }

		public void Next()
		{
            if (_history.Count == 0) return;
			if (_currentlySelectedHistoryItem == null) _currentlySelectedHistoryItem = _history.First;
			else if (_currentlySelectedHistoryItem.Next == null) return;
            else _currentlySelectedHistoryItem = _currentlySelectedHistoryItem.Next;
			_historyEventListener.HistoryItemSelected(_currentlySelectedHistoryItem.Value);
		}

		public void Previous()
		{
			if (_currentlySelectedHistoryItem == null) return;
			if (_currentlySelectedHistoryItem.Previous == null) return;
			_currentlySelectedHistoryItem = _currentlySelectedHistoryItem.Previous;
			_historyEventListener.HistoryItemSelected(_currentlySelectedHistoryItem.Value);
		}
	}
}
