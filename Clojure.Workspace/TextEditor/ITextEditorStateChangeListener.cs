namespace Clojure.Workspace.TextEditor
{
	public interface ITextEditorStateChangeListener
	{
		void OnTextEditorStateChange(TextEditorSnapshot snapshot);
	}
}