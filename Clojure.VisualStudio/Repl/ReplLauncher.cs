using System.Collections.Generic;
using System.IO;
using Clojure.System.Diagnostics;
using Clojure.VisualStudio.Project;
using Microsoft.VisualStudio.Shell.Interop;

namespace Clojure.VisualStudio.Repl
{
	public class ReplLauncher : IProjectUserCommandListener
	{
		private readonly ReplFactory _replFactory;
		private readonly IVsWindowFrame _toolWindowFrame;

		public ReplLauncher(ReplFactory replFactory, IVsWindowFrame toolWindowFrame)
		{
			_replFactory = replFactory;
			_toolWindowFrame = toolWindowFrame;
		}

		public void LaunchRepl(ProjectSnapshot projectSnapshot)
		{
			var environmentVariables = new Dictionary<string, string>();
			environmentVariables["clojure.load.path"] = Path.GetDirectoryName(projectSnapshot.Path);
			var replExecutablePath = "\"" + projectSnapshot.FrameworkPath + "\\Clojure.Main.exe\"";

			var process = new ConsoleProcess(replExecutablePath, environmentVariables);

			_replFactory.CreateRepl(process);
			_toolWindowFrame.Show();
		}
	}
}