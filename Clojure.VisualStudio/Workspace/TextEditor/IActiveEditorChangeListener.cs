using Clojure.VisualStudio.Workspace.TextEditor;

namespace Clojure.Workspace.TextEditor
{
	public interface IActiveEditorChangeListener
	{
		void OnActiveEditorChange(VisualStudioClojureTextEditor editor);
	}
}