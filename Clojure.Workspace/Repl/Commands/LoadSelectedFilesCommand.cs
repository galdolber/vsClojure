using System.Collections.Generic;
using System.Linq;
using Clojure.Workspace.Explorer;
using Clojure.Workspace.Menus;
using Clojure.Workspace.Repl.Presentation;

namespace Clojure.Workspace.Repl.Commands
{
	public class LoadSelectedFilesCommand : IMenuCommandListener, IExplorerSelectionChangedListener, IReplActivationListener
	{
		private List<SolutionItem> _selectedItems;
		private IRepl _repl;

		public void OnMenuCommandClick()
		{
			_repl.LoadFiles(_selectedItems.FindAll(i => i.ItemType == SolutionItemType.File).Select(i => i.Path).ToList());
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
