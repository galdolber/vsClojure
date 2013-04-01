using System.Windows.Input;

namespace Clojure.Base.CommandWindow.EventHandlers
{
	public class UpdateSelectionEventHandler : IKeyEventHandler
	{
		private readonly ITextCommandListener _textCommandListener;

		public UpdateSelectionEventHandler(ITextCommandListener textCommandListener)
		{
			_textCommandListener = textCommandListener;
		}

		public bool CanHandle(CommandWindowUserEvent commandWindowUserEvent)
		{
			return commandWindowUserEvent.IsCursorAfterPrompt() && commandWindowUserEvent.KeyPressed == Key.Home &&
			       commandWindowUserEvent.ShiftDown;
		}

		public void Handle(CommandWindowUserEvent commandWindowUserEvent)
		{
			_textCommandListener.UpdateSelectionToIncludeTextFromCursorPositionToPrompt();
		}
	}
}