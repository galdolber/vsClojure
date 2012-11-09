using System;
using System.Collections.Generic;
using System.Linq;
using Clojure.Workspace.Explorer;
using Clojure.Workspace.Explorer.Menus;

namespace Clojure.Workspace.Repl.Commands
{
	public class LoadSelectedFilesCommand : IExplorerMenuCommandListener
	{
		private readonly IRepl _repl;

		public LoadSelectedFilesCommand(IRepl repl)
		{
			_repl = repl;
		}

		public void OnClick(List<SolutionItem> selectedItems)
		{
			_repl.LoadFiles(selectedItems.FindAll(i => i.ItemType == SolutionItemType.File).Select(i => i.Path).ToList());
		}
	}
}