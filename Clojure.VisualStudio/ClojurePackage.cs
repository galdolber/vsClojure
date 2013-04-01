using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Clojure.Code.Editing.PartialUpdate;
using Clojure.Base.IO.Compression;
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
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Project;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.Win32;
using Process = System.Diagnostics.Process;
using Thread = System.Threading.Thread;

namespace Clojure.VisualStudio
{
	[Guid(PackageGuid)]
	[PackageRegistration(UseManagedResourcesOnly = true)]
	[DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\11.0")]
	[ProvideObject(typeof (GeneralPropertyPage))]
	[ProvideProjectFactory(typeof (ClojureProjectFactory), "Clojure", "Clojure Project Files (*.cljproj);*.cljproj", "cljproj", "cljproj", @"Project\Templates\Projects\Clojure", LanguageVsTemplate = "Clojure", NewProjectRequireNewFolderVsTemplate = false)]
	[ProvideProjectItem(typeof (ClojureProjectFactory), "Clojure Items", @"Project\Templates\ProjectItems\Clojure", 500)]
	[ProvideMenuResource("Menus.ctmenu", 1)]
	[ProvideToolWindow(typeof (ReplToolWindow))]
    [ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string)]
	public sealed class ClojurePackage : ProjectPackage
	{
		public const string PackageGuid = "7712178c-977f-45ec-adf6-e38108cc7739";
        private const string VSCLOJURE_RUNTIMES_DIR = "VSCLOJURE_RUNTIMES_DIR";
        private const string CLOJURE_LOAD_PATH = "CLOJURE_LOAD_PATH";
        private const string VERSION = "1.5.0";
        private const bool OPTIMIZE_COMPILED_JAVASCRIPT = false;

        public static string ClojureLoadPathEnvironmentVariable
        {
            get { return Environment.GetEnvironmentVariable(CLOJURE_LOAD_PATH); }
            set { Environment.SetEnvironmentVariable(CLOJURE_LOAD_PATH, value, EnvironmentVariableTarget.User); }
        }

        public static string VsClojureRuntimesDirEnvironmentVariable
        {
            get { return Environment.GetEnvironmentVariable(VSCLOJURE_RUNTIMES_DIR); }
            set { Environment.SetEnvironmentVariable(VSCLOJURE_RUNTIMES_DIR, value, EnvironmentVariableTarget.User); }
        }


		private DTEEvents _dteEvents;

		private ReplTabControl _replTabControl;
		private ClojureEnvironment _clojureEnvironment;
		private ClojureEditorCollection _editorCollection;
		private ClojureEditorMenuCommandService _menuCommandService;

		protected override void Initialize()
		{
			base.Initialize();
			var dte = (DTE2) GetService(typeof (DTE));
			_dteEvents = dte.Events.DTEEvents;

			_dteEvents.OnStartupComplete +=
				() =>
				{
					_menuCommandService = new ClojureEditorMenuCommandService(this);
					RegisterMenuCommandService(_menuCommandService);

					_editorCollection = new ClojureEditorCollection(dte);
					_editorCollection.AddEditorChangeListener(_menuCommandService);

					var replToolWindow = (ReplToolWindow) FindToolWindow(typeof (ReplToolWindow), 0, true);
					_replTabControl = replToolWindow.Content as ReplTabControl;

					_clojureEnvironment = new ClojureEnvironment();
					_replTabControl.AddReplActivationListener(_clojureEnvironment);

					AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainAssemblyResolve;
					RegisterProjectFactory(new ClojureProjectFactory(this));
					CreateReplMenuCommands();
					EnableTokenizationOfNewClojureBuffers();
					UnzipRuntimes();
                    EnableSettingOfRuntimePathForNewClojureProjects();
				};
		}

		private void RegisterMenuCommandService(OleMenuCommandService menuCommandService)
		{
			var commandRegistry = GetService(typeof(SVsRegisterPriorityCommandTarget)) as IVsRegisterPriorityCommandTarget;
			uint cookie = 0;
			commandRegistry.RegisterPriorityCommandTarget(0, menuCommandService, out cookie);
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
				errorMessage.AppendLine("Failed to extract Clojure runtime(s).  You may need to reinstall vsClojure.");
				errorMessage.AppendLine(e.Message);
                MessageBox.Show(errorMessage.ToString());
			}
		}

		private string GetDirectoryOfDeployedContents()
		{
			string codebaseRegistryLocation = ApplicationRegistryRoot + "\\Packages\\{" + PackageGuid + "}";
			return Path.GetDirectoryName(Registry.GetValue(codebaseRegistryLocation, "CodeBase", "").ToString());
		}

		private void EnableSettingOfRuntimePathForNewClojureProjects()
		{
            string deployDirectory = GetDirectoryOfDeployedContents();
            string runtimePath = deployDirectory + "\\Runtimes";
            string clrRuntimePath1_5_0 = runtimePath + "\\ClojureCLR-1.5.0";

            bool runtimePathIncorrect = VsClojureRuntimesDirEnvironmentVariable != runtimePath;
            if (runtimePathIncorrect)
            {
                VsClojureRuntimesDirEnvironmentVariable = runtimePath;
            }

            string extensionsDirectory = Directory.GetParent(deployDirectory).FullName;

            string clojureLoadPath = ClojureLoadPathEnvironmentVariable ?? "";
            List<string> loadPaths = clojureLoadPath.Split(new[] { Path.PathSeparator }, StringSplitOptions.RemoveEmptyEntries).Where(x => !x.Contains(extensionsDirectory)).ToList();
            loadPaths.Insert(0, clrRuntimePath1_5_0);
            string newClojureLoadPath = loadPaths.Aggregate((x, y) => x + Path.PathSeparator + y);

            bool clojureLoadPathIncorrect = ClojureLoadPathEnvironmentVariable != newClojureLoadPath;
            if (clojureLoadPathIncorrect)
            {
                ClojureLoadPathEnvironmentVariable = newClojureLoadPath;
            }

            if (runtimePathIncorrect || clojureLoadPathIncorrect)
			{
				MessageBox.Show("Setup of vsClojure complete.  Please restart Visual Studio.", "vsClojure Setup");
                if (MessageBox.Show("Would you like to view the vsClojure ReadMe.txt", "vsClojure Readme.txt", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    string pathToReadme = deployDirectory + "\\ReadMe.txt";
                    Process.Start("notepad.exe", pathToReadme);
                }
			}
		}

		private IMenuCommand CreateVisualStudioMenuCommand(CommandID commandId, IExternalClickListener clickListener)
		{
			var menuCommandService = (OleMenuCommandService) GetService(typeof (IMenuCommandService));
			var menuCommandAdapterReference = new MenuCommandAdapterReference();
			var internalMenuCommand = new MenuCommand((o, e) => menuCommandAdapterReference.Adapter.OnClick(), commandId);
			var menuCommandAdapter = new VisualStudioClojureMenuCommandAdapter(internalMenuCommand);
			menuCommandAdapterReference.Adapter = menuCommandAdapter;
			menuCommandService.AddCommand(internalMenuCommand);
			menuCommandAdapter.AddClickListener(clickListener);
			return menuCommandAdapter;
		}

        private Dictionary<string, Process> filesBeingCompiled = new Dictionary<string, Process>();
        private object filesBeingCompiledLock = new object();
	    private VisualStudioTextEditor _textEditor;

	    private void CompileClojureScript(string filePath, string inputFileContents, Action<string> outputResult)
        {
            new Thread(() =>
            {
                outputResult("/* compiling ... */");

                string runtimeDir = string.Format("{0}\\{1}-{2}", VsClojureRuntimesDirEnvironmentVariable, "ClojureScript", VERSION);
                List<string> paths = Directory.GetFiles(string.Format("{0}\\lib\\", runtimeDir), "*.jar", SearchOption.AllDirectories).ToList();
                paths.Add(string.Format("{0}\\src\\clj", runtimeDir));
                paths.Add(string.Format("{0}\\src\\cljs", runtimeDir));
                paths.Add(string.Format("{0}\\lib", runtimeDir));

                string classPath = paths.Aggregate((x, y) => x + ";" + y);
                string compilerPath = String.Format("{0}\\bin\\cljsc.clj", runtimeDir);

                string inputFileName = Path.GetTempFileName();
                using (StreamWriter outfile = new StreamWriter(inputFileName))
                {
                    outfile.Write(inputFileContents);
                }

                string workingDirectory = GetTempDirectory();

                Process newProcess = new Process();
                newProcess.StartInfo.UseShellExecute = false;
                newProcess.StartInfo.RedirectStandardOutput = true;
                newProcess.StartInfo.RedirectStandardError = true;
                newProcess.StartInfo.CreateNoWindow = true;
                newProcess.StartInfo.FileName = "java";
                const string optimizations = OPTIMIZE_COMPILED_JAVASCRIPT ? "{:optimizations :advanced}" : "";
                string arguments = string.Format("-server -cp \"{0}\" clojure.main \"{1}\" \"{2}\" {3}", classPath, compilerPath, inputFileName, optimizations);
                newProcess.StartInfo.Arguments = arguments;
                newProcess.StartInfo.WorkingDirectory = workingDirectory;

                string standardOutput = "";
                string standardError = "";
                lock (filesBeingCompiledLock)
                {
                    if (filesBeingCompiled.ContainsKey(filePath))
                    {
                        Process oldProcess = filesBeingCompiled[filePath];
                        try
                        {
                            oldProcess.Kill();
                        }
                        catch { }
                    }

                    filesBeingCompiled[filePath] = newProcess;

                    IntPtr oldWow64Redirection = new IntPtr();
                    Win32Api.Wow64DisableWow64FsRedirection(ref oldWow64Redirection);

                    try
                    {
                        newProcess.Start();
                    }
                    catch (Exception e)
                    {
                        standardError = e.Message + Environment.NewLine + "Ensure you have the latest version of Java and the JDK installed from Oracle.com" + Environment.NewLine + "Ensure the directory containing java is on the path environment variable (usually C:\\Program Files (x86)\\Java\\jre7\\bin)";
                    }

                    Win32Api.Wow64RevertWow64FsRedirection(oldWow64Redirection);
                }

                if (string.IsNullOrWhiteSpace(standardError))
                {
                    standardOutput = newProcess.StandardOutput.ReadToEnd();
                    standardError = newProcess.StandardError.ReadToEnd();

                    newProcess.WaitForExit();
                }

                if (!string.IsNullOrWhiteSpace(standardError))
                {
                    standardError = string.Format("/*{0}{1}{0}*/{0}", Environment.NewLine, standardError);
                }

                if (!OPTIMIZE_COMPILED_JAVASCRIPT && !string.IsNullOrWhiteSpace(standardOutput))
                {
                    string outDirectory = workingDirectory + "\\out";
                    if (Directory.Exists(outDirectory))
                    {
                        string outputFile = Directory.GetFiles(outDirectory, "*.js", SearchOption.TopDirectoryOnly).FirstOrDefault();
                        string outputFileContent = !string.IsNullOrWhiteSpace(outputFile) ? File.ReadAllText(outputFile) : "";
                        standardOutput = string.Format("/*{0}{1}{0}*/{0}{2}", Environment.NewLine, standardOutput, outputFileContent);
                    }
                    else
                    {
                        standardOutput = string.Format("/*{0}{1}{0}*/{0}", Environment.NewLine, standardOutput);
                    }
                }

                if (!string.IsNullOrWhiteSpace(standardError) || !string.IsNullOrWhiteSpace(standardOutput))
                {
                    outputResult(string.Format("{0}{1}", standardError, standardOutput));
                }
            }).Start();
        }

        private string GetTempDirectory()
        {
            string tempFile = Path.GetTempFileName();
            File.Delete(tempFile);
            string result = Path.ChangeExtension(tempFile, "");
            result = result.Substring(0, result.Length - 1); // remove final .
            Directory.CreateDirectory(result);
            return result;
        }

        private void RequestCompile(ITextDocument textDocument)
        {
            string filePath = textDocument.FilePath;
            DTE2 dte = (DTE2)GetService(typeof(DTE));
            ProjectItem projectItem = dte.Solution.FindProjectItem(filePath);
            if (projectItem == null || projectItem.ContainingProject == null)
            {
                return;
            }

            CompileClojureScript(filePath, textDocument.TextBuffer.CurrentSnapshot.GetText(), compilationResult =>
            {
                string outputFilePath = filePath + ".js";
                if (dte.SourceControl != null && dte.SourceControl.IsItemUnderSCC(outputFilePath) &&
                        !dte.SourceControl.IsItemCheckedOut(outputFilePath))
                {
                    dte.SourceControl.CheckOutItem(outputFilePath);
                }

                File.WriteAllText(outputFilePath, compilationResult);

                if (projectItem.ProjectItems != null &&
                        !projectItem.ProjectItems.Cast<ProjectItem>().Any(x => x.FileNames[0] == outputFilePath))
                {
                    projectItem.ProjectItems.AddFromFile(outputFilePath);
                }
                else
                {
                    projectItem.ContainingProject.ProjectItems.AddFromFile(outputFilePath);
                    ProjectItem newProjectItem = dte.Solution.FindProjectItem(outputFilePath);
                }
            });
        }
        
		private void EnableTokenizationOfNewClojureBuffers()
		{
			var componentModel = (IComponentModel) GetService(typeof (SComponentModel));
			var documentFactoryService = componentModel.GetService<ITextDocumentFactoryService>();
			var editorFactoryService = componentModel.GetService<ITextEditorFactoryService>();

			documentFactoryService.TextDocumentDisposed += (o, e) => { };

			documentFactoryService.TextDocumentCreated +=
				(o, e) =>
				{
				    bool isClojureCLR = e.TextDocument.FilePath.EndsWith(".clj");
				    bool isClojureScript = e.TextDocument.FilePath.EndsWith(".cljs");
				    if (!isClojureCLR && !isClojureScript)
					{
					    return;
					}
					
					var vsClojureTextBuffer = new VisualStudioClojureTextBuffer(e.TextDocument.TextBuffer);
					vsClojureTextBuffer.InvalidateTokens();

					if (isClojureScript)
					{
						e.TextDocument.FileActionOccurred += (sender, fileActionEvent) =>
						{
							if (fileActionEvent.FileActionType == FileActionTypes.ContentSavedToDisk)
							{
								RequestCompile(e.TextDocument);
							}
						};
					}
				};

			editorFactoryService.TextViewCreated +=
				(o, e) =>
				{
                    if (!e.TextView.TextSnapshot.ContentType.TypeName.ToLower().StartsWith("clojure"))
					{
					    return;
					}

					var vsTextBuffer = e.TextView.TextBuffer;

                    if (!vsTextBuffer.Properties.ContainsProperty(typeof(VisualStudioClojureTextBuffer)) || !vsTextBuffer.Properties.ContainsProperty(typeof(ITextDocument)))
                    {
                        return;
                    }

					var clojureTextBuffer = vsTextBuffer.Properties.GetProperty<VisualStudioClojureTextBuffer>(typeof(VisualStudioClojureTextBuffer));
					var filePath = vsTextBuffer.Properties.GetProperty<ITextDocument>(typeof (ITextDocument)).FilePath;

					var editor = new VisualStudioClojureTextView(e.TextView);
					editor.AddUserActionListener(clojureTextBuffer);
					_editorCollection.EditorAdded(filePath, editor);

					IEditorOptions editorOptions = componentModel.GetService<IEditorOptionsFactoryService>().GetOptions(e.TextView);
					editorOptions.SetOptionValue(new ConvertTabsToSpaces().Key, true);
					editorOptions.SetOptionValue(new IndentSize().Key, 2);
				};

			var routingTextEditor = new RoutingTextView();
			_editorCollection.AddEditorChangeListener(routingTextEditor);
			_menuCommandService.Add(new MenuCommand((o, e) => routingTextEditor.Format(), CommandIDs.FormatDocument));
			_menuCommandService.Add(new MenuCommand((o, e) => routingTextEditor.CommentSelectedLines(), CommandIDs.BlockComment));
			_menuCommandService.Add(new MenuCommand((o, e) => routingTextEditor.UncommentSelectedLines(), CommandIDs.BlockUncomment));
		}

		private void CreateReplMenuCommands()
		{
			var dte = (DTE2) GetService(typeof (DTE));
            _textEditor = new VisualStudioTextEditor(dte, (IComponentModel)GetService(typeof(SComponentModel)));
			var replToolWindow = (ReplToolWindow) FindToolWindow(typeof (ReplToolWindow), 0, true);

			var replPortfolio = new ReplPortfolio();
			replPortfolio.AddPortfolioListener(_replTabControl);
			replPortfolio.AddPortfolioListener(replToolWindow);

			var replLauncher = new ReplLauncher(replPortfolio);
            CreateVisualStudioMenuCommand(CommandIDs.StartReplUsingProjectVersion, new ProjectMenuCommand(dte.ToolWindows.SolutionExplorer, replLauncher));

			var explorer = new VisualStudioExplorer(dte);
			var repl = new ReplCommandRouter();
			_replTabControl.AddReplActivationListener(repl);

			var loadSelectedProjectCommand = new LoadSelectedProjectCommand(explorer, repl);
			explorer.AddSelectionListener(loadSelectedProjectCommand);

			var loadSelectedFilesCommand = new LoadSelectedFilesCommand(repl);
			explorer.AddSelectionListener(loadSelectedFilesCommand);

			var explorerMenuCommandCollection = new MenuCommandCollection(MenuCommandCollection.VisibleEditorStates);
			explorerMenuCommandCollection.Add(CreateVisualStudioMenuCommand(CommandIDs.LoadProjectIntoActiveRepl, loadSelectedProjectCommand));
			explorerMenuCommandCollection.Add(CreateVisualStudioMenuCommand(CommandIDs.LoadFileIntoActiveRepl, loadSelectedFilesCommand));
			_clojureEnvironment.AddActivationListener(explorerMenuCommandCollection);

			var loadActiveFileCommand = new LoadActiveFileCommand(repl);
            _textEditor.AddStateChangeListener(loadActiveFileCommand);

			var changeNamespaceCommand = new ChangeNamespaceCommand(repl);
            _textEditor.AddStateChangeListener(changeNamespaceCommand);

			var loadSelectionCommand = new LoadSelectionCommand(repl);
            _textEditor.AddStateChangeListener(loadSelectionCommand);

			var editorMenuCommandCollection = new MenuCommandCollection(MenuCommandCollection.VisibleEditorStates);
			editorMenuCommandCollection.Add(CreateVisualStudioMenuCommand(CommandIDs.LoadActiveDocumentIntoRepl, loadActiveFileCommand));
			editorMenuCommandCollection.Add(CreateVisualStudioMenuCommand(CommandIDs.SwitchReplNamespaceToActiveDocument, changeNamespaceCommand));
			editorMenuCommandCollection.Add(CreateVisualStudioMenuCommand(CommandIDs.LoadSelectedTextIntoRepl, loadSelectionCommand));
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