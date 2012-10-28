using System;
using System.Collections.Generic;

namespace Clojure.Code.Repl
{
	public interface IRepl
	{
		event Action OnClientWrite;
		void Write(string expression);
		void LoadFiles(List<string> fileList);
		void ChangeNamespace(string newNamespace);
		void Stop();
	}
}