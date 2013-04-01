using System.Windows.Input;

namespace Clojure.Base.CommandWindow.EventHandlers
{
	public class MoveCursorToStartOfPromptEventHandler : IKeyEventHandler
	{
		private readonly ITextCommandListener _textCommandListener;

		public MoveCursorToStartOfPromptEventHandler(ITextCommandListener textCommandListener)
		{
			_textCommandListener = textCommandListener;
		}

		public bool CanHandle(CommandWindowUserEvent commandWindowUserEvent)
		{
			return commandWindowUserEvent.IsCursorAfterPrompt() && commandWindowUserEvent.KeyPressed == Key.Home &&
			       !commandWindowUserEvent.ShiftDown;
		}

		public void Handle(CommandWindowUserEvent commandWindowUserEvent)
		{
			_textCommandListener.MoveCursorToStartOfPrompt();
		}
	}
}