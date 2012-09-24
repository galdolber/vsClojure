namespace Clojure.System
{
	public interface IProvider<T>
	{
		T Get();
	}
}