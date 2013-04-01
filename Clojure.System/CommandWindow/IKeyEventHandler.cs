namespace Clojure.Base.CommandWindow
{
	public interface IKeyEventHandler
	{
		bool CanHandle(CommandWindowUserEvent commandWindowUserEvent);
		void Handle(CommandWindowUserEvent commandWindowUserEvent);
	}
}