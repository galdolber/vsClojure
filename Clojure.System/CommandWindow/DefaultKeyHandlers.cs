using System.Collections.Generic;
using Clojure.System.CommandWindow.EventHandlers;

namespace Clojure.System.CommandWindow
{
	public static class DefaultKeyHandlers
	{
		public static void PreventEditingBeforePrompt(this IKeyEventDispatcher dispatcher)
		{
			dispatcher.AddKeyHandler(new BeforePromptEventHandler());
		}

		public static void AddHistoryKeyHandlers(this IHistoryEventListener historyEventListener, IKeyEventDispatcher dispatcher)
		{
			var history = new History(historyEventListener);
			dispatcher.AddKeyHandler(new HistoryNextEventHandler(history));
			dispatcher.AddKeyHandler(new HistoryPreviousEventHandler(history));
			dispatcher.AddKeyHandler(new SubmitEventHandler(history));
		}

		public static void AddSubmitKeyHandlers(this ISubmitCommandListener submitListener, IKeyEventDispatcher dispatcher)
		{
			dispatcher.AddKeyHandler(new SubmitEventHandler(submitListener));
		}

		public static void AddTextEditingKeyHandlers(this ITextCommandListener textCommandListener, IKeyEventDispatcher dispatcher)
		{
			dispatcher.AddKeyHandler(new EraseSelectionEventHandler(textCommandListener));
			dispatcher.AddKeyHandler(new MoveCursorToStartOfPromptEventHandler(textCommandListener));
			dispatcher.AddKeyHandler(new UpdateSelectionEventHandler(textCommandListener));
		}
	}
}