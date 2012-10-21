using System.Collections.Generic;
using System.Windows.Input;

namespace Clojure.System.CommandWindow.EventHandlers
{
	public class HistoryNextEventHandler : IKeyEventHandler
	{
		private readonly List<IHistoryCommandListener> _historyCommandListeners;

		public HistoryNextEventHandler(List<IHistoryCommandListener> historyCommandListeners)
		{
			_historyCommandListeners = historyCommandListeners;
		}

		public bool CanHandle(CommandWindowUserEvent commandWindowUserEvent)
		{
			return commandWindowUserEvent.IsCursortAtOrAfterPrompt() && commandWindowUserEvent.KeyPressed == Key.Down && commandWindowUserEvent.ControlDown;
		}

		public void Handle(CommandWindowUserEvent commandWindowUserEvent)
		{
			_historyCommandListeners.ForEach(l => l.Next());
		}
	}
}