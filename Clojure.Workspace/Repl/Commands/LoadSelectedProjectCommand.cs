using System;
using System.Collections.Generic;
using Clojure.Workspace.Explorer;
using Clojure.Workspace.Menus;

namespace Clojure.Workspace.Repl.Commands
{
	public class LoadSelectedProjectCommand : IExplorerSelectionChangedListener, IExternalClickListener
	{
		private readonly IExplorer _explorer;
		private readonly IRepl _repl;
		private List<SolutionItem> _selectedItems;

		public LoadSelectedProjectCommand(IExplorer explorer, ReplCommandRouter repl)
		{
			_explorer = explorer;
			_repl = repl;
		}

		public void ExplorerSelectionChanged(List<SolutionItem> selectedItems)
		{
			_selectedItems = selectedItems;
		}

		public void OnExternalClick()
		{
			var filePaths = new List<string>();

			foreach (var solutionItem in _selectedItems)
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