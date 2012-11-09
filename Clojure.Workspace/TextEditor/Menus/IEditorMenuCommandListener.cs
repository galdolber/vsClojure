namespace Clojure.Workspace.TextEditor
{
	public interface IEditorMenuCommandListener
	{
		void Selected(TextEditorSnapshot snapshot);
	}
}