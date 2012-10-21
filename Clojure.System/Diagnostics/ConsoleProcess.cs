using System;
using System.Diagnostics;
using Clojure.System.IO.Streams;

namespace Clojure.System.Diagnostics
{
	public class ConsoleProcess : IProcess
	{
		private readonly Process _process;
		private readonly AsynchronousProcessStreamReader _processOutputReader;
		public event Action<string> TextReceived;

		public ConsoleProcess(string replExecutablePath, string projectPath)
		{
			_process = new Process();
			_process.StartInfo = new ProcessStartInfo();
			_process.EnableRaisingEvents = true;
			_process.StartInfo.RedirectStandardOutput = true;
			_process.StartInfo.RedirectStandardInput = true;
			_process.StartInfo.RedirectStandardError = true;
			_process.StartInfo.CreateNoWindow = true;
			_process.StartInfo.UseShellExecute = false;
			_process.StartInfo.FileName = "\"" + replExecutablePath + "\\Clojure.Main.exe\"";
			_process.StartInfo.EnvironmentVariables["clojure.load.path"] = projectPath;
			_processOutputReader = new AsynchronousProcessStreamReader(_process);
			_processOutputReader.DataReceived += TextReceived;
		}

		public void Start()
		{
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