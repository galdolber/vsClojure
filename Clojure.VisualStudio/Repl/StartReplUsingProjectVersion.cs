using System;
using System.Collections.Generic;
using System.IO;
using Clojure.System.Diagnostics;
using Microsoft.VisualStudio.Shell.Interop;

namespace Clojure.VisualStudio.Repl.Operations
{
	public class StartReplUsingProjectVersion
	{
		private readonly ReplFactory _replFactory;
		private readonly IVsWindowFrame _toolWindowFrame;
		private readonly Func<string> _frameworkProvider;
		private readonly Func<EnvDTE.Project> _selectedProjectProvider;

		public StartReplUsingProjectVersion(
			ReplFactory replFactory,
			IVsWindowFrame toolWindowFrame,
			Func<string> frameworkProvider,
			Func<EnvDTE.Project> selectedProjectProvider)
		{
			_frameworkProvider = frameworkProvider;
			_selectedProjectProvider = selectedProjectProvider;
			_replFactory = replFactory;
			_toolWindowFrame = toolWindowFrame;
		}

		public void Execute()
		{
			var environmentVariables = new Dictionary<string, string>();
			environmentVariables["clojure.load.path"] = Path.GetDirectoryName(_selectedProjectProvider().FullName);
			var replExecutablePath = "\"" + _frameworkProvider() + "\\Clojure.Main.exe\"";
			var process = new ConsoleProcess(replExecutablePath, environmentVariables);

			_replFactory.CreateRepl(process);
			_toolWindowFrame.Show();
		}
	}
}