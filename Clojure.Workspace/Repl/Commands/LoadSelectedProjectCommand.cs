using System;
using System.Collections.Generic;
using Clojure.Workspace.Explorer;
using Clojure.Workspace.Explorer.Menus;

namespace Clojure.Workspace.Repl.Commands
{
	public class LoadSelectedProjectCommand : IExplorerMenuCommandListener
	{
		private readonly IExplorer _explorer;
		private readonly IRepl _repl;

		public LoadSelectedProjectCommand(IExplorer explorer, ReplCommandRouter repl)
		{
			_explorer = explorer;
			_repl = repl;
		}

		public void OnClick(List<SolutionItem> selectedItems)
		{
			var filePaths = new List<string>();

			foreach (var solutionItem in selectedItems)
			{
				if (solutionItem.ItemType == SolutionItemType.Project)
				{
					_explorer.FindProjectFiles(solutionItem).ForEach(i => filePaths.Add(i.Path));
				}
				else
				{
					filePaths.Add(solutionItem.Path);
				}
			}

			_repl.LoadFiles(filePaths);
		}
	}
}