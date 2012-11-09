namespace Clojure.Workspace
{
	public interface IEnvironmentListener
	{
		void EnvironmentStateChange(ClojureEnvironmentSnapshot snapshot);
	}
}