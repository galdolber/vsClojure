using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Windows.Controls;
using Clojure.Code.Repl;
using Clojure.System.CommandWindow.Presentation;
using Clojure.System.Diagnostics;
using Clojure.VisualStudio.Editor;
using Clojure.VisualStudio.Repl.Commands;
using Clojure.VisualStudio.Repl.Presentation;
using Clojure.VisualStudio.SolutionExplorer;
using Clojure.VisualStudio.Workspace;
using Clojure.VisualStudio.Workspace.EditorWindow;
using Clojure.VisualStudio.Workspace.TextEditor;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Clojure.VisualStudio.Repl
{
	public class ReplLauncher : IProjectMenuCommandListener
	{
		private readonly IVsWindowFrame _toolWindowFrame;
		private readonly TabControl _replManager;
		private readonly IServiceProvider _serviceProvider;
		private readonly IReplWriteCompleteListener _replWriteCompleteListener;

		public ReplLauncher(IServiceProvider serviceProvider, IVsWindowFrame toolWindowFrame, TabControl replManager, IReplWriteCompleteListener replWriteCompleteListener)
		{
			_replWriteCompleteListener = replWriteCompleteListener;
			_serviceProvider = serviceProvider;
			_replManager = replManager;
			_toolWindowFrame = toolWindowFrame;
		}

		public void Selected(ProjectSnapshot projectSnapshot)
		{
			var environmentVariables = new Dictionary<string, string>();
			environmentVariables["clojure.load.path"] = Path.GetDirectoryName(projectSnapshot.Path);
			var replExecutablePath = "\"" + projectSnapshot.FrameworkPath + "\\Clojure.Main.exe\"";

			CreateRepl(new ConsoleProcess(replExecutablePath, environmentVariables));
			_toolWindowFrame.Show();
		}

		private void CreateRepl(ConsoleProcess replProcess)
		{
			var dte = (DTE2) _serviceProvider.GetService(typeof (DTE));

			var interactiveText = ReplUserInterfaceFactory.CreateInteractiveText();
			var closeButton = ReplUserInterfaceFactory.CreateCloseButton();
			var name = ReplUserInterfaceFactory.CreateTabLabel();
			var grid = ReplUserInterfaceFactory.CreateTextBoxGrid(interactiveText);
			var headerPanel = ReplUserInterfaceFactory.CreateHeaderPanel(name, closeButton);
			var tabItem = ReplUserInterfaceFactory.CreateTabItem(headerPanel, grid);
			var commandWindow = new CommandTextBox(interactiveText);

			var repl = new ExternalProcessRepl(replProcess, commandWindow);
			repl.AddReplWriteCompleteListener(_replWriteCompleteListener);

			var environmentListener = new ClojureEnvironment(tabItem);
			var textEditorWindow = new TextEditorWindow(dte);
			textEditorWindow.AddTextEditorDocumentChangedListener(environmentListener);

			var componentModel = (IComponentModel)_serviceProvider.GetService(typeof(SComponentModel));
			var textEditor = new ClojureTextEditor(componentModel.GetService<IVsEditorAdaptersFactoryService>(), (IVsTextManager)_serviceProvider.GetService(typeof(SVsTextManager)));
			textEditorWindow.AddTextEditorDocumentChangedListener(textEditor);

			_replManager.SelectionChanged += (sender, eventData) => environmentListener.OnReplActivated();
			WireUpTheReplEditorCommandsToTheEditor(new VisualStudioExplorer(dte), repl, environmentListener, textEditorWindow, textEditor);

			closeButton.Click += (o, e) => replProcess.Kill();
			closeButton.Click += (o, e) => _replManager.Items.Remove(tabItem);
			tabItem.Loaded += (o, e) => replProcess.Start();
			_replManager.Items.Add(tabItem);
			_replManager.SelectedItem = tabItem;
		}

		private void WireUpTheReplEditorCommandsToTheEditor(
			IExplorer explorer,
			IReplWriteRequestListener repl,
			ClojureEnvironment environmentListener,
			TextEditorWindow textEditorWindow,
			ClojureTextEditor editor)
		{
			var menuCommandService = (OleMenuCommandService) _serviceProvider.GetService(typeof (IMenuCommandService));

			var loadSelectedProjectCommand = new LoadSelectedProjectCommand(explorer, repl);
			var loadSelectedProjectMenuCommand = new ClojureMenuCommand(ClojureMenuCommand.LoadProjectIntoReplCommandId, loadSelectedProjectCommand);
			loadSelectedProjectMenuCommand.RegisterWith(menuCommandService);
			explorer.AddSelectionListener(loadSelectedProjectCommand);

			var loadSelectedFilesCommand = new LoadSelectedFilesCommand(repl);
			var loadSelectedFilesMenuCommand = new ClojureMenuCommand(new CommandID(Guids.GuidClojureExtensionCmdSet, 12), loadSelectedFilesCommand);
			environmentListener.AddActivationListener(loadSelectedFilesMenuCommand);
			explorer.AddSelectionListener(loadSelectedFilesCommand);
			loadSelectedFilesMenuCommand.RegisterWith(menuCommandService);

			var loadActiveFileCommand = new LoadActiveFileCommand(repl);
			var loadActiveFileMenuCommand = new ClojureMenuCommand(new CommandID(Guids.GuidClojureExtensionCmdSet, 13), loadActiveFileCommand);
			environmentListener.AddActivationListener(loadActiveFileMenuCommand);
			loadActiveFileMenuCommand.RegisterWith(menuCommandService);
			textEditorWindow.AddTextEditorDocumentChangedListener(loadActiveFileCommand);

			var changeNamespaceCommand = new ChangeNamespaceCommand(repl);
			var changeNamespaceMenuCommand = new ClojureMenuCommand(new CommandID(Guids.GuidClojureExtensionCmdSet, 14), changeNamespaceCommand);
			environmentListener.AddActivationListener(changeNamespaceMenuCommand);
			editor.AddStateChangeListener(changeNamespaceCommand);
			changeNamespaceMenuCommand.RegisterWith(menuCommandService);

			var loadSelectionCommand = new LoadSelectionCommand(repl);
			var loadSelectionMenuCommand = new ClojureMenuCommand(new CommandID(Guids.GuidClojureExtensionCmdSet, 15), loadSelectionCommand);
			environmentListener.AddActivationListener(loadSelectionMenuCommand);
			loadSelectionMenuCommand.RegisterWith(menuCommandService);
			editor.AddStateChangeListener(loadSelectionCommand);
		}
	}
}