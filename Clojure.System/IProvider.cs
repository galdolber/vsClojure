namespace Clojure.Base
{
	public interface IProvider<T>
	{
		T Get();
	}
}