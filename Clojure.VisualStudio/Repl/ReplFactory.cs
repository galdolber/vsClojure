using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Windows.Controls;
using Clojure.Code.Parsing;
using Clojure.Code.Repl;
using Clojure.System.Collections;
using Clojure.System.CommandWindow;
using Clojure.System.Diagnostics;
using Clojure.VisualStudio.Editor;
using Clojure.VisualStudio.Menus;
using Clojure.VisualStudio.Project.FileSystem;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Clojure.VisualStudio.Repl
{
	public class ReplFactory
	{
		private readonly TabControl _replManager;
		private readonly IVsWindowFrame _replToolWindow;
		private readonly IServiceProvider _serviceProvider;

		public ReplFactory(TabControl replManager, IVsWindowFrame replToolWindow, IServiceProvider serviceProvider)
		{
			_replManager = replManager;
			_replToolWindow = replToolWindow;
			_serviceProvider = serviceProvider;
		}

		public void CreateRepl(ConsoleProcess replProcess)
		{
			var interactiveText = ReplUserInterfaceFactory.CreateInteractiveText();
			var closeButton = ReplUserInterfaceFactory.CreateCloseButton();
			var name = ReplUserInterfaceFactory.CreateTabLabel();
			var grid = ReplUserInterfaceFactory.CreateTextBoxGrid(interactiveText);
			var headerPanel = ReplUserInterfaceFactory.CreateHeaderPanel(name, closeButton);
			var tabItem = ReplUserInterfaceFactory.CreateTabItem(headerPanel, grid);
			var commandWindow = new CommandTextBox(interactiveText, new List<IKeyEventHandler>());
			var repl = new ExternalProcessRepl(replProcess, commandWindow);

			WireUpTheReplEditorCommandsToTheEditor(repl, tabItem);

			closeButton.Click += (o, e) => repl.Stop();
			closeButton.Click += (o, e) => _replManager.Items.Remove(tabItem);

			_replManager.Items.Add(tabItem);
			_replManager.SelectedItem = tabItem;
		}

		private void WireUpTheReplEditorCommandsToTheEditor(IRepl repl, TabItem tabItem)
		{
			var dte = (DTE2) _serviceProvider.GetService(typeof (DTE));
			var menuCommandService = (OleMenuCommandService) _serviceProvider.GetService(typeof (IMenuCommandService));

			var menuCommandListWirer = new MenuCommandListWirer(
				menuCommandService,
				CreateMenuCommands(repl),
				() => dte.ActiveDocument != null && dte.ActiveDocument.FullName.ToLower().EndsWith(".clj") && _replManager.SelectedItem == tabItem);

			dte.Events.WindowEvents.WindowActivated += (o, e) => menuCommandListWirer.TryToShowMenuCommands();
			_replManager.SelectionChanged += (sender, eventData) => menuCommandListWirer.TryToShowMenuCommands();
		}

		private List<MenuCommand> CreateMenuCommands(IRepl repl)
		{
			var dte = (DTE2) _serviceProvider.GetService(typeof (DTE));
			repl.OnInvisibleWrite += () => _replToolWindow.ShowNoActivate();

			Action loadSelectedFilesIntoRepl =
				() => dte.ToolWindows.SolutionExplorer.GetSelectedFiles().LoadFilesInto(repl);

			Action loadSelectedProjectIntoRepl =
				() => dte.ToolWindows.SolutionExplorer.GetSelectedProject().GetAllFiles().LoadFilesInto(repl);

			Action loadActiveFileIntoRepl =
				() => dte.ActiveDocument.FullName.SingletonAsList().LoadFilesInto(repl);

			var componentModel = (IComponentModel) _serviceProvider.GetService(typeof (SComponentModel));
			var namespaceParser = new NamespaceParser(NamespaceParser.NamespaceSymbols);

			var activeTextBufferStateProvider =
				new ActiveTextBufferStateProvider(
					componentModel.GetService<IVsEditorAdaptersFactoryService>(),
					(IVsTextManager) _serviceProvider.GetService(typeof (SVsTextManager)));

			var menuCommands = new List<MenuCommand>();
			menuCommands.Add(new MenuCommand((sender, args) => loadSelectedProjectIntoRepl(), new CommandID(Guids.GuidClojureExtensionCmdSet, 11)));
			menuCommands.Add(new MenuCommand((sender, args) => loadSelectedFilesIntoRepl(), new CommandID(Guids.GuidClojureExtensionCmdSet, 12)));
			menuCommands.Add(new MenuCommand((sender, args) => loadActiveFileIntoRepl(), new CommandID(Guids.GuidClojureExtensionCmdSet, 13)));
			menuCommands.Add(new MenuCommand((sender, args) => repl.ChangeNamespace(namespaceParser.Execute(activeTextBufferStateProvider.Get())), new CommandID(Guids.GuidClojureExtensionCmdSet, 14)));
			menuCommands.Add(new MenuCommand((sender, args) => repl.WriteInvisibly((string) dte.ActiveDocument.Selection), new CommandID(Guids.GuidClojureExtensionCmdSet, 15)));
			return menuCommands;
		}
	}
}