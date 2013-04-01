namespace Clojure.Base.CommandWindow
{
	public interface ITextCommandListener
	{
		void UpdateSelectionToIncludeTextFromCursorPositionToPrompt();
		void EraseSelection();
		void MoveCursorToStartOfPrompt();
	}
}
