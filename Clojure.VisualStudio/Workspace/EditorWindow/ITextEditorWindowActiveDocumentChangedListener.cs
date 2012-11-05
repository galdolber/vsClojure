namespace Clojure.VisualStudio.Workspace.EditorWindow
{
	public interface ITextEditorWindowActiveDocumentChangedListener
	{
		void OnActiveDocumentChange(string newDocumentPath);
	}
}