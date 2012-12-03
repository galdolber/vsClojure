namespace Clojure.VisualStudio.Workspace.TextEditor
{
	public interface IActiveEditorChangeListener
	{
		void OnActiveEditorChange(VisualStudioClojureTextView view);
		void NonClojureEditorActivated();
	}
}