/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Clojure.Code.Parsing;
using Clojure.System.IO.Compression;
using Clojure.VisualStudio.Editor;
using Clojure.VisualStudio.Editor.AutoFormat;
using Clojure.VisualStudio.Editor.Commenting;
using Clojure.VisualStudio.Editor.Options;
using Clojure.VisualStudio.Editor.TextBuffer;
using Clojure.VisualStudio.Menus;
using Clojure.VisualStudio.Project;
using Clojure.VisualStudio.Project.Configuration;
using Clojure.VisualStudio.Project.Hierarchy;
using Clojure.VisualStudio.Repl;
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
using Microsoft.VisualStudio.Project;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.Win32;

namespace Clojure.VisualStudio
{
	[Guid(PackageGuid)]
	[PackageRegistration(UseManagedResourcesOnly = true)]
	[DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\10.0")]
	[ProvideObject(typeof (GeneralPropertyPage))]
	[ProvideProjectFactory(typeof (ClojureProjectFactory), "Clojure", "Clojure Project Files (*.cljproj);*.cljproj", "cljproj", "cljproj", @"Project\Templates\Projects\Clojure", LanguageVsTemplate = "Clojure", NewProjectRequireNewFolderVsTemplate = false)]
	[ProvideProjectItem(typeof (ClojureProjectFactory), "Clojure Items", @"Project\Templates\ProjectItems\Clojure", 500)]
	[ProvideMenuResource("Menus.ctmenu", 1)]
	[ProvideToolWindow(typeof (ReplToolWindow))]
	[ProvideAutoLoad(UIContextGuids80.NoSolution)]
	public sealed class ClojurePackage : ProjectPackage
	{
		public const string PackageGuid = "7712178c-977f-45ec-adf6-e38108cc7739";

		private ClearableMenuCommandService _thirdPartyEditorCommands;
		private DTEEvents _dteEvents;

		protected override void Initialize()
		{
			base.Initialize();
			var dte = (DTE2) GetService(typeof (DTE));
			_dteEvents = dte.Events.DTEEvents;

			_dteEvents.OnStartupComplete +=
				() =>
				{
					AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainAssemblyResolve;
					RegisterProjectFactory(new ClojureProjectFactory(this));
					RegisterCommandMenuService();
					HideAllClojureEditorMenuCommands();
					ShowClojureProjectMenuCommands();
					EnableTokenizationOfNewClojureBuffers();
					SetupNewClojureBuffersWithSpacingOptions();
					EnableMenuCommandsOnNewClojureBuffers();
					EnableSettingOfRuntimePathForNewClojureProjects();
					UnzipRuntimes();
				};
		}

		private void UnzipRuntimes()
		{
			try
			{
				var runtimeBasePath = Path.Combine(GetDirectoryOfDeployedContents(), "Runtimes");
				Directory.GetFiles(runtimeBasePath, "*.zip").ToList().ForEach(CompressionExtensions.ExtractZipToFreshSubDirectoryAndDelete);
			}
			catch (Exception e)
			{
				var errorMessage = new StringBuilder();
				errorMessage.AppendLine("Failed to extract ClojureCLR runtime(s).  You may need to reinstall vsClojure.");
				errorMessage.AppendLine(e.Message);
			}
		}

		private string GetDirectoryOfDeployedContents()
		{
			string codebaseRegistryLocation = ApplicationRegistryRoot + "\\Packages\\{" + PackageGuid + "}";
			return Path.GetDirectoryName(Registry.GetValue(codebaseRegistryLocation, "CodeBase", "").ToString());
		}

		private void RegisterCommandMenuService()
		{
			var commandRegistry = GetService(typeof (SVsRegisterPriorityCommandTarget)) as IVsRegisterPriorityCommandTarget;
			_thirdPartyEditorCommands = new ClearableMenuCommandService(this);
			uint cookie = 0;
			commandRegistry.RegisterPriorityCommandTarget(0, _thirdPartyEditorCommands, out cookie);
		}

		private void EnableSettingOfRuntimePathForNewClojureProjects()
		{
			string codebaseRegistryLocation = ApplicationRegistryRoot + "\\Packages\\{" + PackageGuid + "}";
			string runtimePath = Registry.GetValue(codebaseRegistryLocation, "CodeBase", "").ToString();
			runtimePath = Path.GetDirectoryName(runtimePath) + "\\Runtimes\\";

			if (Environment.GetEnvironmentVariable("VSCLOJURE_RUNTIMES_DIR", EnvironmentVariableTarget.User) != runtimePath)
			{
				Environment.SetEnvironmentVariable("VSCLOJURE_RUNTIMES_DIR", runtimePath, EnvironmentVariableTarget.User);
				MessageBox.Show("Setup of vsClojure complete.  Please restart Visual Studio.", "vsClojure Setup");
			}
		}

		private void HideAllClojureEditorMenuCommands()
		{
			var allCommandIds = new List<int>() {11, 12, 13, 14, 15};
			var dte = (DTE2) GetService(typeof (DTE));
			var menuCommandService = (OleMenuCommandService) GetService(typeof (IMenuCommandService));
			var menuCommands = new List<MenuCommand>();
			foreach (var commandId in allCommandIds) menuCommands.Add(new MenuCommand((o, s) => { }, new CommandID(Guids.GuidClojureExtensionCmdSet, commandId)));
			var hider = new MenuCommandListHider(menuCommandService, menuCommands);
			//dte.Events.WindowEvents.WindowActivated += (o, e) => hider.HideMenuCommands();
		}

		private void EnableMenuCommandsOnNewClojureBuffers()
		{
			var componentModel = (IComponentModel) GetService(typeof (SComponentModel));
			var editorFactoryService = componentModel.GetService<ITextEditorFactoryService>();

			editorFactoryService.TextViewCreated +=
				(o, e) => e.TextView.GotAggregateFocus +=
				          (sender, args) =>
				          {
							//_thirdPartyEditorCommands.Clear();
							//if (e.TextView.TextSnapshot.ContentType.TypeName.ToLower() != "clojure") return;

							//var editorOptionsBuilder = new EditorOptionsBuilder(componentModel.GetService<IEditorOptionsFactoryService>().GetOptions(e.TextView));
							//var tokenizedBuffer = TokenizedBufferBuilder.TokenizedBuffers[e.TextView.TextBuffer];
							//var formatter = new AutoFormatter(new TextBufferAdapter(e.TextView), tokenizedBuffer);
							//var blockComment = new BlockCommentAdapter(new TextBufferAdapter(e.TextView));
							//var blockUncomment = new BlockUncommentAdapter(new TextBufferAdapter(e.TextView));
							//_thirdPartyEditorCommands.AddCommand(new MenuCommand((commandSender, commandArgs) => formatter.Format(editorOptionsBuilder.Get()), CommandIDs.FormatDocument));
							//_thirdPartyEditorCommands.AddCommand(new MenuCommand((commandSender, commandArgs) => blockComment.Execute(), CommandIDs.BlockComment));
							//_thirdPartyEditorCommands.AddCommand(new MenuCommand((commandSender, commandArgs) => blockUncomment.Execute(), CommandIDs.BlockUncomment));
							//_thirdPartyEditorCommands.AddCommand(new MenuCommand((commandSender, commandArgs) => { }, CommandIDs.GotoDefinition));
				          };
		}

		private void SetupNewClojureBuffersWithSpacingOptions()
		{
			var componentModel = (IComponentModel) GetService(typeof (SComponentModel));
			var editorFactoryService = componentModel.GetService<ITextEditorFactoryService>();

			editorFactoryService.TextViewCreated +=
				(o, e) =>
				{
					if (e.TextView.TextSnapshot.ContentType.TypeName.ToLower() != "clojure") return;
					IEditorOptions editorOptions = componentModel.GetService<IEditorOptionsFactoryService>().GetOptions(e.TextView);
					editorOptions.SetOptionValue(new ConvertTabsToSpaces().Key, true);
					editorOptions.SetOptionValue(new IndentSize().Key, 2);
				};
		}

		private void EnableTokenizationOfNewClojureBuffers()
		{
			var componentModel = (IComponentModel) GetService(typeof (SComponentModel));
			var tokenizedBufferBuilder = new TokenizedBufferBuilder(new Tokenizer());
			var documentFactoryService = componentModel.GetService<ITextDocumentFactoryService>();

			documentFactoryService.TextDocumentDisposed +=
				(o, e) => tokenizedBufferBuilder.RemoveTokenizedBuffer(e.TextDocument.TextBuffer);

			documentFactoryService.TextDocumentCreated +=
				(o, e) => { if (e.TextDocument.FilePath.EndsWith(".clj")) tokenizedBufferBuilder.CreateTokenizedBuffer(e.TextDocument.TextBuffer); };
		}

		public static ReplTabControl ReplTabControl = new ReplTabControl();

		private void ShowClojureProjectMenuCommands()
		{
			var componentModel = (IComponentModel)this.GetService(typeof(SComponentModel));
			var menuCommandService = (OleMenuCommandService) GetService(typeof (IMenuCommandService));
			var replToolWindow = (ReplToolWindow) FindToolWindow(typeof (ReplToolWindow), 0, true);
			var dte = (DTE2) GetService(typeof (DTE));

			var replPortfolio = new ReplPortfolio();
			replPortfolio.AddPortfolioListener(ReplTabControl);
			replPortfolio.AddPortfolioListener(replToolWindow);

			var replLauncher = new ReplLauncher(replPortfolio);
			var projectMenuCommand = new ProjectMenuCommand(dte.ToolWindows.SolutionExplorer, replLauncher);
			menuCommandService.AddCommand(new MenuCommand((sender, args) => projectMenuCommand.Click(), ProjectMenuCommand.LaunchReplCommandId));

			var explorer = new VisualStudioExplorer(dte);
			var environmentListener = new ClojureEnvironment();
			var textEditor = new ClojureTextEditor(componentModel.GetService<IVsEditorAdaptersFactoryService>(), (IVsTextManager)this.GetService(typeof(SVsTextManager)));
			var textEditorWindow = new TextEditorWindow(dte);
			textEditorWindow.AddActiveDocumentChangedListener(textEditor);
			textEditorWindow.AddActiveDocumentChangedListener(environmentListener);

			ReplTabControl.AddReplActivationListener(environmentListener);

			var loadSelectedProjectCommand = new LoadSelectedProjectCommand(explorer);
			var loadSelectedProjectMenuCommand = new ClojureMenuCommand(ClojureMenuCommand.LoadProjectIntoReplCommandId, loadSelectedProjectCommand);
			loadSelectedProjectMenuCommand.RegisterWith(menuCommandService);
			explorer.AddSelectionListener(loadSelectedProjectCommand);
			ReplTabControl.AddReplActivationListener(loadSelectedProjectCommand);

			var loadSelectedFilesCommand = new LoadSelectedFilesCommand();
			var loadSelectedFilesMenuCommand = new ClojureMenuCommand(new CommandID(Guids.GuidClojureExtensionCmdSet, 12), loadSelectedFilesCommand);
			environmentListener.AddActivationListener(loadSelectedFilesMenuCommand);
			explorer.AddSelectionListener(loadSelectedFilesCommand);
			loadSelectedFilesMenuCommand.RegisterWith(menuCommandService);
			ReplTabControl.AddReplActivationListener(loadSelectedFilesCommand);

			var loadActiveFileCommand = new LoadActiveFileCommand();
			var loadActiveFileMenuCommand = new ClojureMenuCommand(new CommandID(Guids.GuidClojureExtensionCmdSet, 13), loadActiveFileCommand);
			environmentListener.AddActivationListener(loadActiveFileMenuCommand);
			loadActiveFileMenuCommand.RegisterWith(menuCommandService);
			textEditorWindow.AddActiveDocumentChangedListener(loadActiveFileCommand);
			ReplTabControl.AddReplActivationListener(loadActiveFileCommand);

			var changeNamespaceCommand = new ChangeNamespaceCommand();
			var changeNamespaceMenuCommand = new ClojureMenuCommand(new CommandID(Guids.GuidClojureExtensionCmdSet, 14), changeNamespaceCommand);
			environmentListener.AddActivationListener(changeNamespaceMenuCommand);
			textEditor.AddStateChangeListener(changeNamespaceCommand);
			changeNamespaceMenuCommand.RegisterWith(menuCommandService);
			ReplTabControl.AddReplActivationListener(changeNamespaceCommand);

			var loadSelectionCommand = new LoadSelectionCommand();
			var loadSelectionMenuCommand = new ClojureMenuCommand(new CommandID(Guids.GuidClojureExtensionCmdSet, 15), loadSelectionCommand);
			environmentListener.AddActivationListener(loadSelectionMenuCommand);
			loadSelectionMenuCommand.RegisterWith(menuCommandService);
			textEditor.AddStateChangeListener(loadSelectionCommand);
			ReplTabControl.AddReplActivationListener(loadSelectionCommand);
		}

		public override string ProductUserContext
		{
			get { return "ClojureProj"; }
		}

		private static Assembly CurrentDomainAssemblyResolve(object sender, ResolveEventArgs args)
		{
			return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => assembly.FullName == args.Name);
		}
	}
}