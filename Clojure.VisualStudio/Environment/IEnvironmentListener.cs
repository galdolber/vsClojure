namespace Clojure.VisualStudio.Environment
{
	public interface IEnvironmentListener
	{
		void EnvironmentStateChange(ClojureEnvironmentSnapshot snapshot);
	}
}