using System.Collections.Generic;
using System.Windows.Input;

namespace Clojure.Base.CommandWindow.EventHandlers
{
	public class HistoryNextEventHandler : IKeyEventHandler
	{
		private readonly IHistoryCommandListener _historyCommandListener;

		public HistoryNextEventHandler(IHistoryCommandListener historyCommandListener)
		{
			_historyCommandListener = historyCommandListener;
		}

		public bool CanHandle(CommandWindowUserEvent commandWindowUserEvent)
		{
			return commandWindowUserEvent.IsCursortAtOrAfterPrompt() && commandWindowUserEvent.KeyPressed == Key.Up;
		}

		public void Handle(CommandWindowUserEvent commandWindowUserEvent)
		{
			_historyCommandListener.Next();
		}
	}
}