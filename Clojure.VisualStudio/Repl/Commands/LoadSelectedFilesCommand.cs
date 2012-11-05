using System.Collections.Generic;
using System.Linq;
using Clojure.Code.Repl;
using Clojure.VisualStudio.SolutionExplorer;

namespace Clojure.VisualStudio.Repl.Commands
{
	public class LoadSelectedFilesCommand : IMenuCommandListener, IExplorerSelectionChangedListener
	{
		private List<SolutionItem> _selectedItems;
		private readonly IReplWriteRequestListener _replWriteRequestListener;

		public LoadSelectedFilesCommand(IReplWriteRequestListener replWriteRequestListener)
		{
			_replWriteRequestListener = replWriteRequestListener;
		}

		public void OnMenuCommandClick()
		{
			_replWriteRequestListener.LoadFiles(_selectedItems.FindAll(i => i.ItemType == SolutionItemType.File).Select(i => i.Path).ToList());
		}

		public void ExplorerSelectionChanged(List<SolutionItem> selectedItems)
		{
			_selectedItems = selectedItems;
		}
	}
}
