namespace Clojure.Base.CommandWindow
{
	public interface ICommandWindow
	{
		void AddKeyHandler(IKeyEventHandler keyHandler);
		void Write(string output);
	}
}
