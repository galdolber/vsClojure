using System;
using System.Collections.Generic;
using Clojure.System.CommandWindow;
using Clojure.System.Diagnostics;

namespace Clojure.Code.Repl
{
	public class ExternalProcessRepl : IRepl, ISubmitCommandListener
	{
		private readonly IProcess _process;
		public event Action OnClientWrite;

		public ExternalProcessRepl(IProcess process)
		{
			_process = process;
		}

		public void Write(string expression)
		{
			_process.Write(expression);
			if (OnClientWrite != null) OnClientWrite();
		}

		public void Stop()
		{
			_process.Kill();
		}

		public void Start()
		{
			_process.Start();
		}

		public void Submit(string expression)
		{
			Write(expression + "\r\n");
		}
	}
}
