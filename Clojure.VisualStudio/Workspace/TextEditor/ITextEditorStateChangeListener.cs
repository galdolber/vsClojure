namespace Clojure.VisualStudio.Workspace.TextEditor
{
	public interface ITextEditorStateChangeListener
	{
		void OnTextEditorStateChange(TextEditorSnapshot snapshot);
	}
}