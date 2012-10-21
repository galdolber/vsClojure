using System.Collections.Generic;
using System.Windows.Input;

namespace Clojure.System.CommandWindow.EventHandlers
{
	public class SubmitEventHandler : IKeyEventHandler
	{
		private readonly List<ISubmitCommandListener> _submitCommandListeners;

		public SubmitEventHandler(List<ISubmitCommandListener> submitCommandListeners)
		{
			_submitCommandListeners = submitCommandListeners;
		}

		public bool CanHandle(CommandWindowUserEvent commandWindowUserEvent)
		{
			return commandWindowUserEvent.IsCursortAtOrAfterPrompt() && commandWindowUserEvent.KeyPressed == Key.Enter && !commandWindowUserEvent.ShiftDown;
		}

		public void Handle(CommandWindowUserEvent commandWindowUserEvent)
		{
			_submitCommandListeners.ForEach(l => l.Submit(commandWindowUserEvent.Expression));
		}
	}
}