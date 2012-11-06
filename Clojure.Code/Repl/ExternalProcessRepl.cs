using System;
using System.Collections.Generic;
using Clojure.System.Diagnostics;

namespace Clojure.Code.Repl
{
	public class ExternalProcessRepl : IRepl
	{
		private readonly IProcess _process;
		private readonly List<IReplWriteCompleteListener> _writeCompleteListeners;
		private readonly List<IReplOutputListener> _outputListeners;

		public ExternalProcessRepl(IProcess process)
		{
			_process = process;
			_process.TextReceived += NotifyListenersOfOutput;
			_writeCompleteListeners = new List<IReplWriteCompleteListener>();
			_outputListeners = new List<IReplOutputListener>();
		}

		public void AddReplWriteCompleteListener(IReplWriteCompleteListener listener)
		{
			_writeCompleteListeners.Add(listener);
		}

		public void AddReplOutputListener(IReplOutputListener listener)
		{
			_outputListeners.Add(listener);
		}

		public void Start()
		{
			_process.Start();
		}

		public void Stop()
		{
			_process.Kill();
		}

		private void NotifyListenersOfOutput(string text)
		{
			_outputListeners.ForEach(l => l.ReplOutput(text));
		}

		public void Write(string expression)
		{
			_process.Write(expression);
			_writeCompleteListeners.ForEach(l => l.OnReplWriteComplete());
		}

		public void Submit(string expression)
		{
			Write(expression + "\r\n");
		}
	}
}
