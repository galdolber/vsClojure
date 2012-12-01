using System.Collections.Generic;
using Clojure.Code.Editing.PartialUpdate;

namespace Clojure.Workspace.TextEditor
{
	public interface IUserActionListener
	{
		void Edit(List<TextChangeData> changes, string textSnapshot);
		void Format();
		void CommentLines(int startIndex, int endIndex);
		void UncommentLines(int startPosition, int endPosition);
	}
}