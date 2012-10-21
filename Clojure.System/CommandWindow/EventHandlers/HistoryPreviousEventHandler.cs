using System.Collections.Generic;
using System.Windows.Input;

namespace Clojure.System.CommandWindow.EventHandlers
{
	public class HistoryPreviousEventHandler : IKeyEventHandler
	{
		private readonly List<IHistoryCommandListener> _historyCommandListeners;

		public HistoryPreviousEventHandler(List<IHistoryCommandListener> historyCommandListeners)
		{
			_historyCommandListeners = historyCommandListeners;
		}

		public bool CanHandle(CommandWindowUserEvent commandWindowUserEvent)
		{
			return commandWindowUserEvent.IsCursortAtOrAfterPrompt() && commandWindowUserEvent.KeyPressed == Key.Up && commandWindowUserEvent.ControlDown;
		}

		public void Handle(CommandWindowUserEvent commandWindowUserEvent)
		{
			_historyCommandListeners.ForEach(l => l.Previous());
		}
	}
}