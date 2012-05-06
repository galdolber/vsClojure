namespace Clojure.VisualStudio.Utilities
{
	public interface IProvider<T>
	{
		T Get();
	}
}