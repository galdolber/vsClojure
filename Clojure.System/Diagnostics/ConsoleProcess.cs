using System;
using System.Collections.Generic;
using System.Diagnostics;
using Clojure.Base.IO.Streams;

namespace Clojure.Base.Diagnostics
{
	public class ConsoleProcess : IProcess
	{
		private readonly Dictionary<string, string> _environmentVariables;
		private readonly Process _process;
		private readonly AsynchronousProcessStreamReader _processOutputReader;
		public event Action<string> TextReceived;

		public ConsoleProcess(string executablePath, Dictionary<string, string> environmentVariables)
		{
			_environmentVariables = environmentVariables;
			_process = new Process();
			_process.StartInfo = new ProcessStartInfo();
			_process.EnableRaisingEvents = true;
			_process.StartInfo.RedirectStandardOutput = true;
			_process.StartInfo.RedirectStandardInput = true;
			_process.StartInfo.RedirectStandardError = true;
			_process.StartInfo.CreateNoWindow = true;
			_process.StartInfo.UseShellExecute = false;
			_process.StartInfo.FileName = executablePath;
			_processOutputReader = new AsynchronousProcessStreamReader(_process);
			_processOutputReader.DataReceived += (data) => TextReceived(data);
		}

		public void Start()
		{
			foreach (var environmentVariable in _environmentVariables)
				_process.StartInfo.EnvironmentVariables[environmentVariable.Key] = environmentVariable.Value;

			_process.Start();
			_process.StandardInput.AutoFlush = true;
			_processOutputReader.StartReading();
		}

		public void Write(string input)
		{
			_process.StandardInput.Write(input);
		}

		public void Kill()
		{
			_process.Kill();
		}
	}
}