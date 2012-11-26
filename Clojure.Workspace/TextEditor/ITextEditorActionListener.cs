using System.Collections.Generic;
using Clojure.Code.Editing.PartialUpdate;

namespace Clojure.Workspace.TextEditor
{
	public interface ITextEditorActionListener
	{
		void TextChanged(List<TextChangeData> changes, string textSnapshot);
		void Format();
		void CommentSelection(List<string> selectedLines);
	}
}