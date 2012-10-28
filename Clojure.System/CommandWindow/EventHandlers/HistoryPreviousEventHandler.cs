using System.Collections.Generic;
using System.Windows.Input;

namespace Clojure.System.CommandWindow.EventHandlers
{
	public class HistoryPreviousEventHandler : IKeyEventHandler
	{
		private readonly IHistoryCommandListener _historyCommandListener;

		public HistoryPreviousEventHandler(IHistoryCommandListener historyCommandListener)
		{
			_historyCommandListener = historyCommandListener;
		}

		public bool CanHandle(CommandWindowUserEvent commandWindowUserEvent)
		{
			return commandWindowUserEvent.IsCursortAtOrAfterPrompt() && commandWindowUserEvent.KeyPressed == Key.Down && commandWindowUserEvent.ControlDown;
		}

		public void Handle(CommandWindowUserEvent commandWindowUserEvent)
		{
			_historyCommandListener.Previous();
		}
	}
}