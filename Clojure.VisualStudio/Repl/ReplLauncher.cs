using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using Clojure.Code.Repl;
using Clojure.System.CommandWindow.Presentation;
using Clojure.System.Diagnostics;
using Clojure.VisualStudio.Repl.Presentation;
using Clojure.VisualStudio.SolutionExplorer;

namespace Clojure.VisualStudio.Repl
{
	public class ReplLauncher : IProjectMenuCommandListener
	{
		private readonly TabControl _replManager;
		private readonly IReplCollector _collector;

		public ReplLauncher(TabControl replManager, IReplCollector collector)
		{
			_collector = collector;
			_replManager = replManager;
		}

		public void Selected(ProjectSnapshot projectSnapshot)
		{
			var environmentVariables = new Dictionary<string, string>();
			environmentVariables["clojure.load.path"] = Path.GetDirectoryName(projectSnapshot.Path);
			var replExecutablePath = "\"" + projectSnapshot.FrameworkPath + "\\Clojure.Main.exe\"";

			_collector.AddRepl(CreateRepl(new ConsoleProcess(replExecutablePath, environmentVariables)));
		}

		private IRepl CreateRepl(ConsoleProcess replProcess)
		{
			var interactiveText = ReplUserInterfaceFactory.CreateInteractiveText();
			var closeButton = ReplUserInterfaceFactory.CreateCloseButton();
			var name = ReplUserInterfaceFactory.CreateTabLabel();
			var grid = ReplUserInterfaceFactory.CreateTextBoxGrid(interactiveText);
			var headerPanel = ReplUserInterfaceFactory.CreateHeaderPanel(name, closeButton);
			var tabItem = ReplUserInterfaceFactory.CreateTabItem(headerPanel, grid);
			var commandWindow = new CommandTextBox(interactiveText);

			closeButton.Click += (o, e) => replProcess.Kill();
			closeButton.Click += (o, e) => _replManager.Items.Remove(tabItem);
			tabItem.Loaded += (o, e) => replProcess.Start();
			_replManager.Items.Add(tabItem);
			_replManager.SelectedItem = tabItem;

			return new ExternalProcessRepl(replProcess, commandWindow);
		}
	}
}