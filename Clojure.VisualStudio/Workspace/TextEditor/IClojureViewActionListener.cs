namespace Clojure.VisualStudio.Workspace.TextEditor
{
	public interface IClojureViewActionListener
	{
		void OnCaretPositionChange(int newPosition);
	}
}