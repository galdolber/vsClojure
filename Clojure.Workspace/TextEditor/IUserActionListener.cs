using System;

namespace Clojure.Workspace.TextEditor
{
	public interface IUserActionListener
	{
		void Format();
		void CommentLines(int startPosition, int endPosition);
		void UncommentLines(int startPosition, int endPosition);
		void OnCaretPositionChange(int newPosition);
	}
}