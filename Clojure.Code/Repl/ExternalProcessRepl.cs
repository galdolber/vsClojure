using System;
using System.Collections.Generic;
using Clojure.System.CommandWindow;
using Clojure.System.Diagnostics;

namespace Clojure.Code.Repl
{
	public class ExternalProcessRepl : IRepl
	{
		private readonly IProcess _process;
		private readonly ICommandWindow _commandWindow;
		public event Action OnInvisibleWrite;

		public ExternalProcessRepl(IProcess process, ICommandWindow commandWindow)
		{
			_process = process;
			_commandWindow = commandWindow;
			_process.TextReceived += commandWindow.Write;
		}

		public void WriteInvisibly(string expression)
		{
			Write(expression);
			_commandWindow.WriteLine();
			if (OnInvisibleWrite != null) OnInvisibleWrite();
		}

		public void Write(string expression)
		{
			_process.Write(expression);
		}

		public void LoadFiles(List<string> fileList)
		{
			WriteInvisibly(fileList
				.FindAllClojureFiles()
				.CreateScriptToLoadFilesIntoRepl());
		}

		public void ChangeNamespace(string newNamespace)
		{
			WriteInvisibly("(in-ns '" + newNamespace + ")");
		}

		public void Stop()
		{
			_process.Kill();
		}
	}
}
