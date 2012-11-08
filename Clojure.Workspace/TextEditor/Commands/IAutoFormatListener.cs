namespace Clojure.Workspace.TextEditor.Commands
{
	public interface IAutoFormatListener
	{
		void OnAutoFormat(string text);
	}
}