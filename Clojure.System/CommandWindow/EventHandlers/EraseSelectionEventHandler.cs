using System.Windows.Input;

namespace Clojure.Base.CommandWindow.EventHandlers
{
	public class EraseSelectionEventHandler : IKeyEventHandler
	{
		private readonly ITextCommandListener _textCommandListener;

		public EraseSelectionEventHandler(ITextCommandListener textCommandListener)
		{
			_textCommandListener = textCommandListener;
		}

		public bool CanHandle(CommandWindowUserEvent commandWindowUserEvent)
		{
			return commandWindowUserEvent.IsCursorAtPrompt() && commandWindowUserEvent.KeyPressed == Key.Back;
		}

		public void Handle(CommandWindowUserEvent commandWindowUserEvent)
		{
			_textCommandListener.EraseSelection();
		}
	}
}