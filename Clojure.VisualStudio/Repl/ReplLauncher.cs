using System.Collections.Generic;
using System.IO;
using Clojure.Code.Repl;
using Clojure.System.Diagnostics;
using Clojure.VisualStudio.SolutionExplorer;

namespace Clojure.VisualStudio.Repl
{
	public class ReplLauncher : IProjectMenuCommandListener
	{
		private readonly IReplCollector _collector;

		public ReplLauncher(IReplCollector collector)
		{
			_collector = collector;
		}

		public void Selected(ProjectSnapshot projectSnapshot)
		{
			var environmentVariables = new Dictionary<string, string>();
			environmentVariables["clojure.load.path"] = Path.GetDirectoryName(projectSnapshot.Path);
			var replExecutablePath = "\"" + projectSnapshot.FrameworkPath + "\\Clojure.Main.exe\"";
			var process = new ConsoleProcess(replExecutablePath, environmentVariables);
			_collector.AddRepl(new ExternalProcessRepl(process));
		}
	}
}