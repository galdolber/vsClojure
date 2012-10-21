using System.Windows.Input;

namespace Clojure.System.CommandWindow.EventHandlers
{
	public class EraseSelectionEventHandler : IKeyEventHandler
	{
		private readonly ICommandWindow _commandWindow;

		public EraseSelectionEventHandler(ICommandWindow commandWindow)
		{
			_commandWindow = commandWindow;
		}

		public bool CanHandle(CommandWindowUserEvent commandWindowUserEvent)
		{
			return commandWindowUserEvent.IsCursorAtPrompt() && commandWindowUserEvent.KeyPressed == Key.Back;
		}

		public void Handle(CommandWindowUserEvent commandWindowUserEvent)
		{
			_commandWindow.EraseSelection();
		}
	}
}