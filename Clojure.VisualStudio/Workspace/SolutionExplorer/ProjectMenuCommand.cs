using System;
using System.ComponentModel.Design;
using Clojure.VisualStudio.Project.Launching;
using Clojure.Workspace.Explorer;
using Clojure.Workspace.Menus;
using EnvDTE;
using Microsoft.VisualStudio.Project;

namespace Clojure.VisualStudio.Workspace.SolutionExplorer
{
	public class ProjectMenuCommand : IExternalClickListener
	{
		private readonly UIHierarchy _solutionExplorer;
		private readonly IProjectMenuCommandListener _projectMenuCommandListener;

		public ProjectMenuCommand(UIHierarchy solutionExplorer, IProjectMenuCommandListener projectMenuCommandListener)
		{
			_solutionExplorer = solutionExplorer;
			_projectMenuCommandListener = projectMenuCommandListener;
		}

		public void OnExternalClick()
		{
			var selectedProject = _solutionExplorer.GetSelectedProject();
			var projectNode = ((ProjectNode)selectedProject.Object);
			var frameworkPath = projectNode.CreateLaunchParameters().FrameworkPath;
			var projectPath = selectedProject.FullName;
			_projectMenuCommandListener.Selected(new ProjectSnapshot(projectPath, frameworkPath));
		}
	}
}