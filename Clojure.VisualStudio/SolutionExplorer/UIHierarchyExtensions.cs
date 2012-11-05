using System;
using System.Collections.Generic;
using Clojure.VisualStudio.Project.Hierarchy;
using EnvDTE;

namespace Clojure.VisualStudio.SolutionExplorer
{
	public static class UIHierarchyExtensions
	{
		public static List<string> GetSelectedFiles(this UIHierarchy hierarchy)
		{
			Array items = (Array)hierarchy.SelectedItems;
			List<string> selectedFilePaths = new List<string>();

			foreach (UIHierarchyItem item in items)
			{
				ProjectItem projectItem = (ProjectItem)item.Object;
				string filePath = projectItem.Properties.Item("FullPath").Value.ToString();
				selectedFilePaths.Add(filePath);
			}

			return selectedFilePaths;
		}

		public static EnvDTE.Project GetSelectedProject(this UIHierarchy hierarchy)
		{
			Array selectedItems = (Array)hierarchy.SelectedItems;
			UIHierarchyItem selectedItem = (UIHierarchyItem)selectedItems.GetValue(0);
			EnvDTE.Project selectedProject = selectedItem.Object as EnvDTE.Project;

			if (selectedProject == null)
				return ((ProjectItem)selectedItem.Object).ContainingProject;

			return selectedProject;
		}

		public static List<string> GetAllFiles(this EnvDTE.Project project)
		{
			List<string> files = new List<string>();
			if (project == null) return files;

			Queue<ProjectItem> projectItemsToLookAt = new Queue<ProjectItem>();
			foreach (ProjectItem projectItem in project.ProjectItems) projectItemsToLookAt.Enqueue(projectItem);

			while (projectItemsToLookAt.Count > 0)
			{
				ProjectItem currentProjectItem = projectItemsToLookAt.Dequeue();

				if (currentProjectItem.Object.GetType() == typeof(ClojureProjectFileNode))
					files.Add(currentProjectItem.Properties.Item("FullPath").Value.ToString());

				if (currentProjectItem.ProjectItems != null && currentProjectItem.ProjectItems.Count > 0)
					foreach (ProjectItem childItem in currentProjectItem.ProjectItems)
						projectItemsToLookAt.Enqueue(childItem);
			}

			return files;
		}
	}
}
