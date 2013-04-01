using System;

namespace Clojure.Base.Diagnostics
{
	public interface IProcess
	{
		event Action<string> TextReceived;
		void Start();
		void Write(string input);
		void Kill();
	}
}