namespace Clojure.Workspace.TextEditor
{
	public interface IUserActionListener
	{
		void Format();
		void CommentLines(int startIndex, int endIndex);
		void UncommentLines(int startPosition, int endPosition);
		void OnCaretPositionChange(int newPosition);
	}
}