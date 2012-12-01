namespace Clojure.Workspace.TextEditor
{
	public interface IActiveEditorChangeListener
	{
		void OnActiveDocumentChange(IUserActionSource source);
	}
}