using System;
using System.Collections.Generic;
using System.Linq;
using Clojure.Workspace.Explorer;
using Clojure.Workspace.Menus;

namespace Clojure.Workspace.Repl.Commands
{
	public class LoadSelectedFilesCommand : IExplorerSelectionChangedListener, IExternalClickListener
	{
		private readonly IRepl _repl;
		private List<SolutionItem> _selectedItems;

		public LoadSelectedFilesCommand(IRepl repl)
		{
			_repl = repl;
		}

		public void ExplorerSelectionChanged(List<SolutionItem> selectedItems)
		{
			_selectedItems = selectedItems;
		}

		public void OnExternalClick()
		{
			_repl.LoadFiles(_selectedItems.FindAll(i => i.ItemType == SolutionItemType.File).Select(i => i.Path).ToList());
		}
	}
}