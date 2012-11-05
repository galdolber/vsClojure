using System;
using System.Collections.Generic;
using Clojure.System.CommandWindow;
using Clojure.System.Diagnostics;

namespace Clojure.Code.Repl
{
	public class ExternalProcessRepl : IReplWriteCompleteDispatcher, ISubmitCommandListener, IReplWriteRequestListener
	{
		private readonly IProcess _process;
		private readonly List<IReplWriteCompleteListener> _listeners;

		public ExternalProcessRepl(IProcess process, ICommandWindow commandWindow)
		{
			_process = process;
			_process.TextReceived += commandWindow.Write;
			this.AddSubmitKeyHandlers(commandWindow);
			_listeners = new List<IReplWriteCompleteListener>();
		}

		public void AddReplWriteCompleteListener(IReplWriteCompleteListener listener)
		{
			_listeners.Add(listener);
		}

		public void Write(string expression)
		{
			_process.Write(expression);
			_listeners.ForEach(l => l.OnReplWriteComplete());
		}

		public void Submit(string expression)
		{
			Write(expression + "\r\n");
		}
	}
}
