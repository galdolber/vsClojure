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
using Clojure.VisualStudio.Editor;
using Clojure.VisualStudio.Editor.AutoFormat;
using Clojure.VisualStudio.Editor.Commenting;
using Clojure.VisualStudio.Editor.Options;
using Clojure.VisualStudio.Editor.TextBuffer;
using Clojure.VisualStudio.IO.Compression;
using Clojure.VisualStudio.Menus;
using Clojure.VisualStudio.Project;
using Clojure.VisualStudio.Project.Configuration;
using Clojure.VisualStudio.Project.Hierarchy;
using Clojure.VisualStudio.Project.Launching;
using Clojure.VisualStudio.Repl;
using Clojure.VisualStudio.Repl.Operations;
using Clojure.VisualStudio.Utilities;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Project;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
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

		protected override void Initialize()
		{
			base.Initialize();
			var dte = (DTE2) GetService(typeof (DTE));

			dte.Events.DTEEvents.OnStartupComplete +=
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
			IVsRegisterPriorityCommandTarget commandRegistry = GetService(typeof (SVsRegisterPriorityCommandTarget)) as IVsRegisterPriorityCommandTarget;
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
			List<int> allCommandIds = new List<int>() {11, 12, 13, 14, 15};
			DTE2 dte = (DTE2) GetService(typeof (DTE));
			OleMenuCommandService menuCommandService = (OleMenuCommandService) GetService(typeof (IMenuCommandService));
			List<MenuCommand> menuCommands = new List<MenuCommand>();
			foreach (int commandId in allCommandIds) menuCommands.Add(new MenuCommand((o, s) => { }, new CommandID(Guids.GuidClojureExtensionCmdSet, commandId)));
			MenuCommandListHider hider = new MenuCommandListHider(menuCommandService, menuCommands);
			dte.Events.WindowEvents.WindowActivated += (o, e) => hider.HideMenuCommands();
		}

		private void EnableMenuCommandsOnNewClojureBuffers()
		{
			var componentModel = (IComponentModel) GetService(typeof (SComponentModel));
			ITextEditorFactoryService editorFactoryService = componentModel.GetService<ITextEditorFactoryService>();

			editorFactoryService.TextViewCreated += (o, e) => e.TextView.GotAggregateFocus +=
				(sender, args) =>
				{
					_thirdPartyEditorCommands.Clear();
					if (e.TextView.TextSnapshot.ContentType.TypeName.ToLower() != "clojure") return;

					var editorOptionsBuilder = new EditorOptionsBuilder(componentModel.GetService<IEditorOptionsFactoryService>().GetOptions(e.TextView));
					var tokenizedBuffer = TokenizedBufferBuilder.TokenizedBuffers[e.TextView.TextBuffer];
					var formatter = new AutoFormatter(new TextBufferAdapter(e.TextView), tokenizedBuffer);
					var blockComment = new BlockCommentAdapter(new TextBufferAdapter(e.TextView));
					var blockUncomment = new BlockUncommentAdapter(new TextBufferAdapter(e.TextView));
					_thirdPartyEditorCommands.AddCommand(new MenuCommand((commandSender, commandArgs) => formatter.Format(editorOptionsBuilder.Get()), CommandIDs.FormatDocument));
					_thirdPartyEditorCommands.AddCommand(new MenuCommand((commandSender, commandArgs) => blockComment.Execute(), CommandIDs.BlockComment));
					_thirdPartyEditorCommands.AddCommand(new MenuCommand((commandSender, commandArgs) => blockUncomment.Execute(), CommandIDs.BlockUncomment));
					_thirdPartyEditorCommands.AddCommand(new MenuCommand((commandSender, commandArgs) => { }, CommandIDs.GotoDefinition));
				};
		}

		private void SetupNewClojureBuffersWithSpacingOptions()
		{
			var componentModel = (IComponentModel) GetService(typeof (SComponentModel));
			ITextEditorFactoryService editorFactoryService = componentModel.GetService<ITextEditorFactoryService>();

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
			TokenizedBufferBuilder tokenizedBufferBuilder = new TokenizedBufferBuilder(new Tokenizer());
			ITextDocumentFactoryService documentFactoryService = componentModel.GetService<ITextDocumentFactoryService>();

			documentFactoryService.TextDocumentDisposed +=
				(o, e) => tokenizedBufferBuilder.RemoveTokenizedBuffer(e.TextDocument.TextBuffer);

			documentFactoryService.TextDocumentCreated +=
				(o, e) => { if (e.TextDocument.FilePath.EndsWith(".clj")) tokenizedBufferBuilder.CreateTokenizedBuffer(e.TextDocument.TextBuffer); };
		}

		private void ShowClojureProjectMenuCommands()
		{
			OleMenuCommandService menuCommandService = (OleMenuCommandService) GetService(typeof (IMenuCommandService));
			ReplToolWindow replToolWindow = (ReplToolWindow) FindToolWindow(typeof (ReplToolWindow), 0, true);
			IVsWindowFrame replToolWindowFrame = (IVsWindowFrame) replToolWindow.Frame;
			DTE2 dte = (DTE2) GetService(typeof (DTE));
			IProvider<EnvDTE.Project> projectProvider = new SelectedProjectProvider(dte.Solution, dte.ToolWindows.SolutionExplorer);

			menuCommandService.AddCommand(
				new MenuCommand(
					(sender, args) =>
						new StartReplUsingProjectVersion(
							new ReplFactory(replToolWindow.TabControl, replToolWindowFrame, this),
							replToolWindowFrame,
							() => new LaunchParametersBuilder((ProjectNode) projectProvider.Get().Object).Get().FrameworkPath,
							new SelectedProjectProvider(dte.Solution, dte.ToolWindows.SolutionExplorer)).Execute(),
					new CommandID(Guids.GuidClojureExtensionCmdSet, 10)));
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