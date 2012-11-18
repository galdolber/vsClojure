namespace Clojure.Workspace.Menus
{
	public interface IMenuCommand
	{
		void AddClickListener(IExternalClickListener listener);
		void Hide();
		void Show();
	}
}
