using System.Windows.Input;

namespace Clojure.System.CommandWindow.EventHandlers
{
	public class UpdateSelectionEventHandler : IKeyEventHandler
	{
		private readonly ICommandWindow _commandWindow;

		public UpdateSelectionEventHandler(ICommandWindow commandWindow)
		{
			_commandWindow = commandWindow;
		}

		public bool CanHandle(CommandWindowUserEvent commandWindowUserEvent)
		{
			return commandWindowUserEvent.IsCursorAfterPrompt() && commandWindowUserEvent.KeyPressed == Key.Home &&
			       commandWindowUserEvent.ShiftDown;
		}

		public void Handle(CommandWindowUserEvent commandWindowUserEvent)
		{
			_commandWindow.UpdateSelectionToIncludeTextFromCursorPositionToPrompt();
		}
	}
}