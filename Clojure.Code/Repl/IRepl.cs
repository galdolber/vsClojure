using System;
using System.Collections.Generic;

namespace Clojure.Code.Repl
{
	public interface IRepl
	{
		event Action OnClientWrite;
		void Write(string expression);
		void Stop();
	}
}