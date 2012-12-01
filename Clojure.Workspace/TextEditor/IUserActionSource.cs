namespace Clojure.Workspace.TextEditor
{
	public interface IUserActionSource
	{
		void AddUserActionListener(IUserActionListener listener);
	}
}