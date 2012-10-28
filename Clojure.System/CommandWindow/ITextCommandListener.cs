namespace Clojure.System.CommandWindow
{
	public interface ITextCommandListener
	{
		void UpdateSelectionToIncludeTextFromCursorPositionToPrompt();
		void EraseSelection();
		void MoveCursorToStartOfPrompt();
	}
}
