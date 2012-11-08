namespace Clojure.Workspace.TextEditor.Options
{
	public interface IEditorOptionsChangedListener
	{
		void OnOptionChange(EditorOptions newOptions);
	}
}