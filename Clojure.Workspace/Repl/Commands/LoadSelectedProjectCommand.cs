using System.Collections.Generic;
using Clojure.Workspace.Explorer;
using Clojure.Workspace.Menus;
using Clojure.Workspace.Repl.Presentation;

namespace Clojure.Workspace.Repl.Commands
{
	public class LoadSelectedProjectCommand : IMenuCommandListener, IExplorerSelectionChangedListener, IReplActivationListener
	{
		private readonly IExplorer _explorer;
		private List<SolutionItem> _selectedItems;
		private IRepl _repl;

		public LoadSelectedProjectCommand(IExplorer explorer)
		{
			_selectedItems = new List<SolutionItem>();
			_explorer = explorer;
		}

		public void OnMenuCommandClick()
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

		public void ReplActivated(IRepl repl)
		{
			_repl = repl;
		}
	}
}