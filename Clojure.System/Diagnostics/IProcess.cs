using System;

namespace Clojure.System.Diagnostics
{
	public interface IProcess
	{
		event Action<string> TextReceived;
		void Start();
		void Write(string input);
		void Kill();
	}
}