using System.Collections.Generic;
using System.Windows.Input;
using Clojure.Base.IO.Keyboard;

namespace Clojure.Base.CommandWindow.EventHandlers
{
	public class BeforePromptEventHandler : IKeyEventHandler
	{
		private static readonly List<Key> AllowableNonPromptKeys = new List<Key>(KeyboardExaminer.ArrowKeys) { Key.Home, Key.End, Key.PageUp, Key.PageDown };

		public bool CanHandle(CommandWindowUserEvent commandWindowUserEvent)
		{
			return !commandWindowUserEvent.IsCursortAtOrAfterPrompt() && !AllowableNonPromptKeys.Contains(commandWindowUserEvent.KeyPressed);
		}

		public void Handle(CommandWindowUserEvent commandWindowUserEvent)
		{
			
		}
	}
}