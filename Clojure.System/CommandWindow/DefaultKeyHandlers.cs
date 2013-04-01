using System.Collections.Generic;
using Clojure.Base.CommandWindow.EventHandlers;

namespace Clojure.Base.CommandWindow
{
	public static class DefaultKeyHandlers
	{
		public static void PreventEditingBeforePrompt(this ICommandWindow dispatcher)
		{
			dispatcher.AddKeyHandler(new BeforePromptEventHandler());
		}

		public static void AddHistoryKeyHandlers(this IHistoryEventListener historyEventListener, ICommandWindow dispatcher)
		{
			var history = new History(historyEventListener);
			dispatcher.AddKeyHandler(new HistoryNextEventHandler(history));
			dispatcher.AddKeyHandler(new HistoryPreviousEventHandler(history));
			dispatcher.AddKeyHandler(new SubmitEventHandler(history));
		}

		public static void AddSubmitKeyHandlers(this ISubmitCommandListener submitListener, ICommandWindow dispatcher)
		{
			dispatcher.AddKeyHandler(new SubmitEventHandler(submitListener));
		}

		public static void AddTextEditingKeyHandlers(this ITextCommandListener textCommandListener, ICommandWindow dispatcher)
		{
			dispatcher.AddKeyHandler(new EraseSelectionEventHandler(textCommandListener));
			dispatcher.AddKeyHandler(new MoveCursorToStartOfPromptEventHandler(textCommandListener));
			dispatcher.AddKeyHandler(new UpdateSelectionEventHandler(textCommandListener));
		}
	}
}