namespace Clojure.VisualStudio.Workspace.TextEditor.Commands
{
	public interface IAutoFormatListener
	{
		void OnAutoFormat(string text);
	}
}