namespace Clojure.VisualStudio.Workspace
{
	public interface IEnvironmentListener
	{
		void EnvironmentStateChange(ClojureEnvironmentSnapshot snapshot);
	}
}