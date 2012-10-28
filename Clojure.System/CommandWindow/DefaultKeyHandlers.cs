using System.Collections.Generic;
using Clojure.System.CommandWindow.EventHandlers;

namespace Clojure.System.CommandWindow
{
	public static class DefaultKeyHandlers
	{
		public static void PreventEditingBeforePrompt(this List<IKeyEventHandler> keyHandlerList)
		{
			keyHandlerList.Add(new BeforePromptEventHandler());
		}

		public static void AddHistoryKeyHandlers(this IHistoryEventListener historyEventListener, List<IKeyEventHandler> keyHandlerList)
		{
			var history = new History(historyEventListener);
			keyHandlerList.Add(new HistoryNextEventHandler(history));
			keyHandlerList.Add(new HistoryPreviousEventHandler(history));
			keyHandlerList.Add(new SubmitEventHandler(history));
		}

		public static void AddSubmitKeyHandlers(this ISubmitCommandListener submitListener, List<IKeyEventHandler> keyHandlerList)
		{
			keyHandlerList.Add(new SubmitEventHandler(submitListener));
		}

		public static void AddTextEditingKeyHandlers(this ITextCommandListener textCommandListener, List<IKeyEventHandler> keyHandlerList)
		{
			keyHandlerList.Add(new EraseSelectionEventHandler(textCommandListener));
			keyHandlerList.Add(new MoveCursorToStartOfPromptEventHandler(textCommandListener));
			keyHandlerList.Add(new UpdateSelectionEventHandler(textCommandListener));
		}
	}
}