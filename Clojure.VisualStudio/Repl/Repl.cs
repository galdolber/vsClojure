using System;
using System.Collections.Generic;
using System.Diagnostics;
using Clojure.VisualStudio.Repl.Operations;

namespace Clojure.VisualStudio.Repl
{
	public class Repl
	{
		private readonly Process _process;
		private readonly TextBoxWriter _textBoxWriter;
		public event Action OnInvisibleWrite;

		public Repl(Process process, TextBoxWriter textBoxWriter)
		{
			_process = process;
			_textBoxWriter = textBoxWriter;
		}

		public void WriteInvisibly(string expression)
		{
			WriteExpressionToRepl(expression);
			_textBoxWriter.WriteToTextBox("\r\n");
			if (OnInvisibleWrite != null) OnInvisibleWrite();
		}

		public void WriteExpressionToRepl(string expression)
		{
			_process.StandardInput.WriteLine(expression);
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
	}
}