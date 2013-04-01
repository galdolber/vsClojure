namespace Clojure.Workspace.TextEditor
{
	public interface ITextEditorStateChangeListener
	{
		void OnTextEditorStatusChange(TextBufferSnapshot snapshot);
	}
}