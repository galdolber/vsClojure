using System;
using System.IO;
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
			_replFactory.CreateRepl(_frameworkProvider(), Path.GetDirectoryName(_selectedProjectProvider().FullName));
			_toolWindowFrame.Show();
		}
	}
}