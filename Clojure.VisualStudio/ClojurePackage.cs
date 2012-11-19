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
using Clojure.VisualStudio.Project.Configuration;
using Clojure.VisualStudio.Project.Hierarchy;
using Clojure.VisualStudio.Workspace.Menus;
using Clojure.VisualStudio.Workspace.Repl;
using Clojure.VisualStudio.Workspace.SolutionExplorer;
using Clojure.VisualStudio.Workspace.TextEditor;
using Clojure.Workspace;
using Clojure.Workspace.Menus;
using Clojure.Workspace.Repl;
using Clojure.Workspace.Repl.Commands;
using Clojure.Workspace.Repl.Presentation;
using Clojure.Workspace.TextEditor;
using Clojure.Workspace.TextEditor.Commands;
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
		private DTEEvents _dteEvents;

		private ReplTabControl _replTabControl;
		private ClojureEnvironment _clojureEnvironment;
		private TextEditorWindow _textEditorWindow;
		private ClojureTextEditor _textEditor;
		private ClojureTextEditorOptions _textEditorOptions;

		protected override void Initialize()
		{
			base.Initialize();
			var dte = (DTE2) GetService(typeof (DTE));
			_dteEvents = dte.Events.DTEEvents;

			_dteEvents.OnStartupComplete +=
				() =>
				{
					_replTabControl = new ReplTabControl();

					var replToolWindow = (ReplToolWindow)FindToolWindow(typeof(ReplToolWindow), 0, true);
					replToolWindow.SetControl(_replTabControl);

					var componentModel = (IComponentModel)GetService(typeof(SComponentModel));
					_textEditor = new ClojureTextEditor(componentModel.GetService<IVsEditorAdaptersFactoryService>(), (IVsTextManager)this.GetService(typeof(SVsTextManager)));
					_textEditorWindow = new TextEditorWindow(dte);
					_textEditorWindow.AddActiveDocumentChangedListener(_textEditor);
					_textEditorOptions = new ClojureTextEditorOptions(componentModel.GetService<IEditorOptionsFactoryService>());

					_clojureEnvironment = new ClojureEnvironment();
					_textEditorWindow.AddActiveDocumentChangedListener(_clojureEnvironment);
					_replTabControl.AddReplActivationListener(_clojureEnvironment);

					AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainAssemblyResolve;
					RegisterProjectFactory(new ClojureProjectFactory(this));
					CreateReplMenuCommands();
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

		private void EnableMenuCommandsOnNewClojureBuffers()
		{
			var smartIndentCommand = new AutoIndent();
			_textEditorOptions.AddOptionsChangedListener(smartIndentCommand);
			_textEditor.AddStateChangeListener(smartIndentCommand);
			SmartIndentProvider.Command = smartIndentCommand;

			var autoFormatCommand = new AutoFormatCommand(_textEditor);
			_textEditorOptions.AddOptionsChangedListener(autoFormatCommand);
			_textEditor.AddStateChangeListener(autoFormatCommand);

			var blockCommentCommand = new BlockCommentCommand(_textEditor);
			_textEditor.AddStateChangeListener(blockCommentCommand);

			var blockUncommentCommand = new BlockUncommentCommand(_textEditor);
			_textEditor.AddStateChangeListener(blockUncommentCommand);

			var menuCommandCollection = new MenuCommandCollection(MenuCommandCollection.VisibleEditorStates);
			menuCommandCollection.Add(CreateVisualStudioMenuCommand(CommandIDs.FormatDocument, autoFormatCommand));
			menuCommandCollection.Add(CreateVisualStudioMenuCommand(CommandIDs.BlockComment, blockCommentCommand));
			menuCommandCollection.Add(CreateVisualStudioMenuCommand(CommandIDs.BlockUncomment, blockUncommentCommand));
			_clojureEnvironment.AddActivationListener(menuCommandCollection);
		}

		// Factory - VS specific.  Need interface to work with non-VS specific code?
		private IMenuCommand CreateVisualStudioMenuCommand(CommandID commandId, IExternalClickListener clickListener)
		{
			var menuCommandService = (OleMenuCommandService)GetService(typeof(IMenuCommandService));
			var menuCommandAdapterReference = new MenuCommandAdapterReference();
			var internalMenuCommand = new MenuCommand((o, e) => menuCommandAdapterReference.Adapter.OnClick(), commandId);
			var menuCommandAdapter = new VisualStudioClojureMenuCommandAdapter(internalMenuCommand);
			menuCommandAdapterReference.Adapter = menuCommandAdapter;
			menuCommandService.AddCommand(internalMenuCommand);
			menuCommandAdapter.AddClickListener(clickListener);
			return menuCommandAdapter;
		}

		private void SetupNewClojureBuffersWithSpacingOptions()
		{
			var componentModel = (IComponentModel) GetService(typeof (SComponentModel));
			var editorFactoryService = componentModel.GetService<ITextEditorFactoryService>();

			editorFactoryService.TextViewCreated +=
				(o, e) =>
				{
					// Have the ClojureTextEditor set the options each time the active editor changes?
					// One ClojureTextEditor per TextView?  Might need to create a "router" TextEditor to
					// get menu commands to work with multiple ClojureTextEditors.
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

		private void CreateReplMenuCommands()
		{
			var dte = (DTE2) GetService(typeof (DTE));
			var replToolWindow = (ReplToolWindow) FindToolWindow(typeof (ReplToolWindow), 0, true);

			var replPortfolio = new ReplPortfolio();
			replPortfolio.AddPortfolioListener(_replTabControl);
			replPortfolio.AddPortfolioListener(replToolWindow);

			var replLauncher = new ReplLauncher(replPortfolio);
			CreateVisualStudioMenuCommand(ProjectMenuCommand.LaunchReplCommandId, new ProjectMenuCommand(dte.ToolWindows.SolutionExplorer, replLauncher));

			var explorer = new VisualStudioExplorer(dte);
			var repl = new ReplCommandRouter();
			_replTabControl.AddReplActivationListener(repl);

			var loadSelectedProjectCommand = new LoadSelectedProjectCommand(explorer, repl);
			explorer.AddSelectionListener(loadSelectedProjectCommand);

			var loadSelectedFilesCommand = new LoadSelectedFilesCommand(repl);
			explorer.AddSelectionListener(loadSelectedFilesCommand);

			var explorerMenuCommandCollection = new MenuCommandCollection(MenuCommandCollection.VisibleEditorStates);
			explorerMenuCommandCollection.Add(CreateVisualStudioMenuCommand(new CommandID(Guids.GuidClojureExtensionCmdSet, 11), loadSelectedProjectCommand));
			explorerMenuCommandCollection.Add(CreateVisualStudioMenuCommand(new CommandID(Guids.GuidClojureExtensionCmdSet, 12), loadSelectedFilesCommand));
			_clojureEnvironment.AddActivationListener(explorerMenuCommandCollection);

			var loadActiveFileCommand = new LoadActiveFileCommand(repl);
			_textEditor.AddStateChangeListener(loadActiveFileCommand);

			var changeNamespaceCommand = new ChangeNamespaceCommand(repl);
			_textEditor.AddStateChangeListener(changeNamespaceCommand);

			var loadSelectionCommand = new LoadSelectionCommand(repl);
			_textEditor.AddStateChangeListener(loadSelectionCommand);

			var editorMenuCommandCollection = new MenuCommandCollection(MenuCommandCollection.VisibleEditorStates);
			editorMenuCommandCollection.Add(CreateVisualStudioMenuCommand(new CommandID(Guids.GuidClojureExtensionCmdSet, 13), loadActiveFileCommand));
			editorMenuCommandCollection.Add(CreateVisualStudioMenuCommand(new CommandID(Guids.GuidClojureExtensionCmdSet, 14), changeNamespaceCommand));
			editorMenuCommandCollection.Add(CreateVisualStudioMenuCommand(new CommandID(Guids.GuidClojureExtensionCmdSet, 15), loadSelectionCommand));
			_clojureEnvironment.AddActivationListener(editorMenuCommandCollection);
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