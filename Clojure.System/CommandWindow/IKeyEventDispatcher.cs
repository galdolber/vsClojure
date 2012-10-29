namespace Clojure.System.CommandWindow
{
	public interface IKeyEventDispatcher
	{
		void AddKeyHandler(IKeyEventHandler keyHandler);
	}
}
