using System.Collections.Generic;
using Clojure.System.CommandWindow.EventHandlers;

namespace Clojure.System.CommandWindow
{
	public static class ICommandWindowExtensions
	{
		public static List<IKeyEventHandler> AddDefaultKeyHandlers(this ICommandWindow commandWindow, List<IKeyEventHandler> keyHandlerList)
		{
			return new List<IKeyEventHandler>
			       	{
			       		new BeforePromptEventHandler(),
			       		new SubmitEventHandler(new List<ISubmitCommandListener>()),
			       		new HistoryNextEventHandler(new List<IHistoryCommandListener>()),
			       		new HistoryPreviousEventHandler(new List<IHistoryCommandListener> { new History(commandWindow) }),
			       		new EraseSelectionEventHandler(commandWindow),
			       		new MoveCursorToStartOfPromptEventHandler(commandWindow),
			       		new UpdateSelectionEventHandler(commandWindow)
			       	};
		}
	}
}