using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Windows.Controls;
using Clojure.Code.Parsing;
using Clojure.System.Collections;
using Clojure.System.IO.Keyboard;
using Clojure.System.IO.Streams;
using Clojure.System.State;
using Clojure.VisualStudio.Editor;
using Clojure.VisualStudio.Menus;
using Clojure.VisualStudio.Repl.Operations;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Process = System.Diagnostics.Process;
using Thread = System.Threading.Thread;
using Clojure.VisualStudio.Project.FileSystem;

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

		public void CreateRepl(string replPath, string projectPath)
		{
			var interactiveText = ReplUserInterfaceFactory.CreateInteractiveText();
			var closeButton = ReplUserInterfaceFactory.CreateCloseButton();
			var name = ReplUserInterfaceFactory.CreateTabLabel();
			var grid = ReplUserInterfaceFactory.CreateTextBoxGrid(interactiveText);
			var headerPanel = ReplUserInterfaceFactory.CreateHeaderPanel(name, closeButton);
			var tabItem = ReplUserInterfaceFactory.CreateTabItem(headerPanel, grid);
			var replProcess = CreateReplProcess(replPath, projectPath);
			var replEntity = new Entity<ReplState> {CurrentState = new ReplState()};
			var repl = new Repl(replProcess, new TextBoxWriter(interactiveText, replEntity));

			WireUpTheTextBoxInputToTheReplProcess(repl, interactiveText, replEntity);
			WireUpTheOutputOfTheReplProcessToTheTextBox(interactiveText, replProcess, replEntity);
			WireUpTheReplEditorCommandsToTheEditor(repl, tabItem);

			closeButton.Click +=
				(o, e) =>
				{
					replProcess.Kill();
					_replManager.Items.Remove(tabItem);
				};

			_replManager.Items.Add(tabItem);
			_replManager.SelectedItem = tabItem;
		}

		private void WireUpTheReplEditorCommandsToTheEditor(Repl repl, TabItem tabItem)
		{
			var dte = (DTE2)_serviceProvider.GetService(typeof(DTE));

			var menuCommandListWirer = new MenuCommandListWirer(
				(OleMenuCommandService)_serviceProvider.GetService(typeof(IMenuCommandService)),
				CreateMenuCommands(repl),
				() => dte.ActiveDocument != null && dte.ActiveDocument.FullName.ToLower().EndsWith(".clj") && _replManager.SelectedItem == tabItem);

			dte.Events.WindowEvents.WindowActivated += (o, e) => menuCommandListWirer.TryToShowMenuCommands();
			_replManager.SelectionChanged += (sender, eventData) => menuCommandListWirer.TryToShowMenuCommands();
		}

		private static void WireUpTheTextBoxInputToTheReplProcess(Repl repl, TextBox replTextBox, Entity<ReplState> replEntity)
		{
			var inputKeyHandler = new InputKeyHandler(new KeyboardExaminer(), replEntity, replTextBox, repl);
			var history = new History(new KeyboardExaminer(), replEntity, replTextBox);

			replTextBox.PreviewKeyDown += history.PreviewKeyDown;
			replTextBox.PreviewTextInput += inputKeyHandler.PreviewTextInput;
			replTextBox.PreviewKeyDown += inputKeyHandler.PreviewKeyDown;
		}

		private static void WireUpTheOutputOfTheReplProcessToTheTextBox(TextBox replTextBox, Process replProcess, Entity<ReplState> replEntity)
		{
			var standardOutputStream = new StreamBuffer();
			var standardErrorStream = new StreamBuffer();
			var textboxWriter = new TextBoxWriter(replTextBox, replEntity);

			var processStreamReader = new AsynchronousAggregateStreamReader(standardOutputStream, standardErrorStream);
			processStreamReader.DataReceived += textboxWriter.WriteToTextBox;

			var outputReaderThread = new Thread(processStreamReader.StartReading);
			var outputBufferStreamThread = new Thread(() => standardOutputStream.ReadStream(replProcess.StandardOutput.BaseStream));
			var errorBufferStreamThread = new Thread(() => standardOutputStream.ReadStream(replProcess.StandardError.BaseStream));

			replTextBox.Loaded +=
				(o, e) =>
				{
					if (outputReaderThread.IsAlive) return;
					replProcess.Start();
					replProcess.StandardInput.AutoFlush = true;
					outputBufferStreamThread.Start();
					errorBufferStreamThread.Start();
					outputReaderThread.Start();
				};

			replProcess.Exited +=
				(o, e) =>
				{
					outputBufferStreamThread.Abort();
					errorBufferStreamThread.Abort();
					outputReaderThread.Abort();
				};
		}

		private List<MenuCommand> CreateMenuCommands(Repl repl)
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
			menuCommands.Add(new MenuCommand((sender, args) => repl.WriteInvisibly((string)dte.ActiveDocument.Selection), new CommandID(Guids.GuidClojureExtensionCmdSet, 15)));
			return menuCommands;
		}

		private static Process CreateReplProcess(string replPath, string projectPath)
		{
			var process = new Process();
			process.EnableRaisingEvents = true;
			process.StartInfo = new ProcessStartInfo();
			process.StartInfo.FileName = "\"" + replPath + "\\Clojure.Main.exe\"";
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardInput = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.EnvironmentVariables["clojure.load.path"] = projectPath;
			return process;
		}
	}
}