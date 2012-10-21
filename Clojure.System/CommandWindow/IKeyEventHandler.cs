namespace Clojure.System.CommandWindow
{
	public interface IKeyEventHandler
	{
		bool CanHandle(CommandWindowUserEvent commandWindowUserEvent);
		void Handle(CommandWindowUserEvent commandWindowUserEvent);
	}
}