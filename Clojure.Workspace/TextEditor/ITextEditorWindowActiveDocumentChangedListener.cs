namespace Clojure.Workspace.TextEditor
{
	public interface ITextEditorWindowActiveDocumentChangedListener
	{
		void OnActiveDocumentChange(string newDocumentPath);
	}
}