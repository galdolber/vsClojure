using Clojure.Workspace.TextEditor;

namespace Clojure.VisualStudio.Workspace.TextEditor
{
	public class RoutingTextEditor : IActiveEditorChangeListener
	{
		private VisualStudioClojureTextEditor _editor;

		public void Format()
		{
			_editor.Format();
		}

		public void CommentSelectedLines()
		{
			_editor.CommentSelectedLines();
		}

		public void UncommentSelectedLines()
		{
			_editor.UncommentSelectedLines();
		}

		public void OnActiveEditorChange(VisualStudioClojureTextEditor editor)
		{
			_editor = editor;
		}
	}
}
