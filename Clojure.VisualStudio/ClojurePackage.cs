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
using Clojure.Workspace.Explorer.Menus;
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
			// Remove this duplication.
			var componentModel = (IComponentModel) GetService(typeof (SComponentModel));
			var textEditorOptions = new ClojureTextEditorOptions(componentModel.GetService<IEditorOptionsFactoryService>());
			var smartIndentCommand = new AutoIndent();
			textEditorOptions.AddOptionsChangedListener(smartIndentCommand);
			SmartIndentProvider.Command = smartIndentCommand;

			CreateEditorMenuCommand(CommandIDs.FormatDocument).AddMenuCommandListener(new AutoFormatCommand(_textEditor));
			CreateEditorMenuCommand(CommandIDs.BlockComment).AddMenuCommandListener(new BlockCommentCommand(_textEditor));
			CreateEditorMenuCommand(CommandIDs.BlockUncomment).AddMenuCommandListener(new BlockUncommentCommand(_textEditor));

			// Group editor options with other text editor snapshot data.
		}

		private EditorMenuCommand CreateEditorMenuCommand(CommandID commandId)
		{
			var menuCommandAdapter = CreateMenuCommandAdapter(commandId, EnvironmentVisibility.VisibleEditorStates);
			var editorMenuCommand = new EditorMenuCommand();
			menuCommandAdapter.AddClickListener(editorMenuCommand);
			return editorMenuCommand;
		}

		private IMenuCommand CreateMenuCommandAdapter(CommandID commandId, List<ClojureEnvironmentState> visibleStates)
		{
			var editorEnvironmentListener = new EnvironmentVisibility(visibleStates);
			_clojureEnvironment.AddActivationListener(editorEnvironmentListener);

			var menuCommandAdapter = CreateVisualStudioMenuCommand(commandId);

			editorEnvironmentListener.AddVisibilityListener(menuCommandAdapter);
			return menuCommandAdapter;
		}

		// Factory - VS specific.  Need interface to work with non-VS specific code?
		private IMenuCommand CreateVisualStudioMenuCommand(CommandID commandId)
		{
			var menuCommandService = (OleMenuCommandService)GetService(typeof(IMenuCommandService));
			var menuCommandAdapterReference = new MenuCommandAdapterReference();
			var internalMenuCommand = new MenuCommand((o, e) => menuCommandAdapterReference.Adapter.OnClick(), commandId);
			var menuCommandAdapter = new VisualStudioClojureMenuCommandAdapter(internalMenuCommand);
			menuCommandAdapterReference.Adapter = menuCommandAdapter;
			menuCommandService.AddCommand(internalMenuCommand);
			return menuCommandAdapter;
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

		private IMenuCommand CreateReplMenuCommand(CommandID commandId)
		{
			return CreateMenuCommandAdapter(commandId, EnvironmentVisibility.VisibleReplStates);
		}

		private EditorMenuCommand CreateReplEditorMenuCommand(CommandID commandId)
		{
			var adapter = CreateReplMenuCommand(commandId);
			var editorMenuCommand = new EditorMenuCommand();
			adapter.AddClickListener(editorMenuCommand);
			return editorMenuCommand;
		}

		private ExplorerMenuCommand CreateExplorerMenuCommand(CommandID commandId)
		{
			var adapter = CreateReplMenuCommand(commandId);
			var explorerMenuCommand = new ExplorerMenuCommand();
			adapter.AddClickListener(explorerMenuCommand);
			return explorerMenuCommand;
		}

		private void CreateReplMenuCommands()
		{
			var dte = (DTE2) GetService(typeof (DTE));
			var menuCommandService = (OleMenuCommandService) GetService(typeof (IMenuCommandService));
			var replToolWindow = (ReplToolWindow) FindToolWindow(typeof (ReplToolWindow), 0, true);

			var replPortfolio = new ReplPortfolio();
			replPortfolio.AddPortfolioListener(_replTabControl);
			replPortfolio.AddPortfolioListener(replToolWindow);

			var replLauncher = new ReplLauncher(replPortfolio);
			var projectMenuCommand = new ProjectMenuCommand(dte.ToolWindows.SolutionExplorer, replLauncher);
			menuCommandService.AddCommand(new MenuCommand((sender, args) => projectMenuCommand.Click(), ProjectMenuCommand.LaunchReplCommandId));

			var explorer = new VisualStudioExplorer(dte);
			var repl = new ReplCommandRouter();
			_replTabControl.AddReplActivationListener(repl);

			CreateExplorerMenuCommand(new CommandID(Guids.GuidClojureExtensionCmdSet, 11)).AddClickListener(new LoadSelectedProjectCommand(explorer, repl));
			CreateExplorerMenuCommand(new CommandID(Guids.GuidClojureExtensionCmdSet, 12)).AddClickListener(new LoadSelectedFilesCommand(repl));
			CreateReplEditorMenuCommand(new CommandID(Guids.GuidClojureExtensionCmdSet, 13)).AddMenuCommandListener(new LoadActiveFileCommand(repl));
			CreateReplEditorMenuCommand(new CommandID(Guids.GuidClojureExtensionCmdSet, 14)).AddMenuCommandListener(new ChangeNamespaceCommand(repl));
			CreateReplEditorMenuCommand(new CommandID(Guids.GuidClojureExtensionCmdSet, 15)).AddMenuCommandListener(new LoadSelectionCommand(repl));
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