using System.Collections.Generic;
using System.Windows.Input;

namespace Clojure.Base.CommandWindow.EventHandlers
{
	public class SubmitEventHandler : IKeyEventHandler
	{
		private readonly ISubmitCommandListener _submitCommandListener;

		public SubmitEventHandler(ISubmitCommandListener submitCommandListener)
		{
			_submitCommandListener = submitCommandListener;
		}

		public bool CanHandle(CommandWindowUserEvent commandWindowUserEvent)
		{
			return commandWindowUserEvent.IsCursortAtOrAfterPrompt() && commandWindowUserEvent.KeyPressed == Key.Enter && !commandWindowUserEvent.ShiftDown;
		}

		public void Handle(CommandWindowUserEvent commandWindowUserEvent)
		{
			_submitCommandListener.Submit(commandWindowUserEvent.Expression);
		}
	}
}