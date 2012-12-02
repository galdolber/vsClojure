namespace Clojure.VisualStudio.Workspace.TextEditor
{
	public class RoutingTextView : IActiveEditorChangeListener
	{
		private VisualStudioClojureTextView _view;

		public void Format()
		{
			_view.Format();
		}

		public void CommentSelectedLines()
		{
			_view.CommentSelectedLines();
		}

		public void UncommentSelectedLines()
		{
			_view.UncommentSelectedLines();
		}

		public void OnActiveEditorChange(VisualStudioClojureTextView view)
		{
			_view = view;
		}
	}
}