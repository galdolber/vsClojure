using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Windows.Controls;
using Clojure.Code.Parsing;
using Clojure.Code.Repl;
using Clojure.System.Collections;
using Clojure.System.CommandWindow;
using Clojure.System.CommandWindow.Presentation;
using Clojure.System.Diagnostics;
using Clojure.VisualStudio.Editor;
using Clojure.VisualStudio.Menus;
using Clojure.VisualStudio.Project;
using Clojure.VisualStudio.Project.Hierarchy;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Clojure.VisualStudio.Repl
{
	public class ReplLauncher : IProjectUserCommandListener
	{
		private readonly IVsWindowFrame _toolWindowFrame;
		private readonly TabControl _replManager;
		private readonly IServiceProvider _serviceProvider;

		public ReplLauncher(IServiceProvider serviceProvider, IVsWindowFrame toolWindowFrame, TabControl replManager)
		{
			_serviceProvider = serviceProvider;
			_replManager = replManager;
			_toolWindowFrame = toolWindowFrame;
		}

		public void LaunchRepl(ProjectSnapshot projectSnapshot)
		{
			var environmentVariables = new Dictionary<string, string>();
			environmentVariables["clojure.load.path"] = Path.GetDirectoryName(projectSnapshot.Path);
			var replExecutablePath = "\"" + projectSnapshot.FrameworkPath + "\\Clojure.Main.exe\"";

			CreateRepl(new ConsoleProcess(replExecutablePath, environmentVariables));
			_toolWindowFrame.Show();
		}

		public void CreateRepl(ConsoleProcess replProcess)
		{
			var interactiveText = ReplUserInterfaceFactory.CreateInteractiveText();
			var closeButton = ReplUserInterfaceFactory.CreateCloseButton();
			var name = ReplUserInterfaceFactory.CreateTabLabel();
			var grid = ReplUserInterfaceFactory.CreateTextBoxGrid(interactiveText);
			var headerPanel = ReplUserInterfaceFactory.CreateHeaderPanel(name, closeButton);
			var tabItem = ReplUserInterfaceFactory.CreateTabItem(headerPanel, grid);

			var commandWindow = new CommandTextBox(interactiveText);
			replProcess.TextReceived += commandWindow.Write;

			var repl = new ExternalProcessRepl(replProcess);
			repl.AddSubmitKeyHandlers(commandWindow);

			WireUpTheReplEditorCommandsToTheEditor(repl, tabItem);

			closeButton.Click += (o, e) => repl.Stop();
			closeButton.Click += (o, e) => _replManager.Items.Remove(tabItem);
			tabItem.Loaded += (o, e) => repl.Start();
			_replManager.Items.Add(tabItem);
			_replManager.SelectedItem = tabItem;
		}

		private void WireUpTheReplEditorCommandsToTheEditor(IRepl repl, TabItem tabItem)
		{
			var dte = (DTE2)_serviceProvider.GetService(typeof(DTE));
			var menuCommandService = (OleMenuCommandService)_serviceProvider.GetService(typeof(IMenuCommandService));

			var menuCommandListWirer = new MenuCommandListWirer(
				menuCommandService,
				CreateMenuCommands(repl),
				() => dte.ActiveDocument != null && dte.ActiveDocument.FullName.ToLower().EndsWith(".clj") && _replManager.SelectedItem == tabItem);

			dte.Events.WindowEvents.WindowActivated += (o, e) => menuCommandListWirer.TryToShowMenuCommands();
			_replManager.SelectionChanged += (sender, eventData) => menuCommandListWirer.TryToShowMenuCommands();
		}

		private List<MenuCommand> CreateMenuCommands(IRepl repl)
		{
			var dte = (DTE2)_serviceProvider.GetService(typeof(DTE));
			repl.OnClientWrite += () => _toolWindowFrame.ShowNoActivate();

			var componentModel = (IComponentModel)_serviceProvider.GetService(typeof(SComponentModel));

			var activeTextBufferStateProvider =
				new ActiveTextBufferStateProvider(
					componentModel.GetService<IVsEditorAdaptersFactoryService>(),
					(IVsTextManager)_serviceProvider.GetService(typeof(SVsTextManager)));

			var menuCommands = new List<MenuCommand>();
			menuCommands.Add(new MenuCommand((sender, args) => repl.LoadFiles(dte.ToolWindows.SolutionExplorer.GetSelectedProject().GetAllFiles()), new CommandID(Guids.GuidClojureExtensionCmdSet, 11)));
			menuCommands.Add(new MenuCommand((sender, args) => repl.LoadFiles(dte.ToolWindows.SolutionExplorer.GetSelectedFiles()), new CommandID(Guids.GuidClojureExtensionCmdSet, 12)));
			menuCommands.Add(new MenuCommand((sender, args) => repl.LoadFiles(dte.ActiveDocument.FullName.SingletonAsList()), new CommandID(Guids.GuidClojureExtensionCmdSet, 13)));
			menuCommands.Add(new MenuCommand((sender, args) => repl.ChangeNamespace(activeTextBufferStateProvider.Get()), new CommandID(Guids.GuidClojureExtensionCmdSet, 14)));
			menuCommands.Add(new MenuCommand((sender, args) => repl.Write((string)dte.ActiveDocument.Selection), new CommandID(Guids.GuidClojureExtensionCmdSet, 15)));
			return menuCommands;
		}
	}
}