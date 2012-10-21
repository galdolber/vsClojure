namespace Clojure.System.CommandWindow
{
	public interface ICommandWindow
	{
		void ReplaceCurrentExpressionWith(string value);
		void MoveCursorToStartOfPrompt();
		void UpdateSelectionToIncludeTextFromCursorPositionToPrompt();
		void EraseSelection();
		void Write(string output);
		void WriteLine();
	}
}