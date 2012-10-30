using System;
using Clojure.System.CommandWindow;
using Clojure.System.Diagnostics;

namespace Clojure.Code.Repl
{
	public class ExternalProcessRepl : IRepl, ISubmitCommandListener
	{
		private readonly IProcess _process;
		public event Action OnClientWrite;

		public ExternalProcessRepl(IProcess process, ICommandWindow commandWindow)
		{
			_process = process;
			_process.TextReceived += commandWindow.Write;
			this.AddSubmitKeyHandlers(commandWindow);
		}

		public void Write(string expression)
		{
			_process.Write(expression);
			if (OnClientWrite != null) OnClientWrite();
		}

		public void Submit(string expression)
		{
			Write(expression + "\r\n");
		}
	}
}
