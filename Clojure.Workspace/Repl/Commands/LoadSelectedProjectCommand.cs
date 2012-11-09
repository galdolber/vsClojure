using System.Collections.Generic;
using Clojure.Workspace.Explorer;
using Clojure.Workspace.Menus;

namespace Clojure.Workspace.Repl.Commands
{
	public class LoadSelectedProjectCommand : IExternalClickListener, IExplorerSelectionChangedListener
	{
		private readonly IExplorer _explorer;
		private List<SolutionItem> _selectedItems;
		private readonly IRepl _repl;

		public LoadSelectedProjectCommand(IExplorer explorer, ReplCommandRouter repl)
		{
			_selectedItems = new List<SolutionItem>();
			_explorer = explorer;
			_repl = repl;
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

		public void ExplorerSelectionChanged(List<SolutionItem> selectedItems)
		{
			_selectedItems = selectedItems;
		}
	}
}