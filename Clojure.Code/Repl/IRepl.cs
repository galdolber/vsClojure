using System;

namespace Clojure.Code.Repl
{
	public interface IRepl
	{
		event Action OnClientWrite;
		void Write(string expression);
	}
}