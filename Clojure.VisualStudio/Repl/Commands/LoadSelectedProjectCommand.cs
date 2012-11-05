using System.Collections.Generic;
using Clojure.Code.Repl;
using Clojure.VisualStudio.SolutionExplorer;

namespace Clojure.VisualStudio.Repl.Commands
{
	public class LoadSelectedProjectCommand : IMenuCommandListener, IExplorerSelectionChangedListener
	{
		private readonly IExplorer _explorer;
		private readonly IReplWriteRequestListener _requestListener;
		private List<SolutionItem> _selectedItems;

		public LoadSelectedProjectCommand(IExplorer explorer, IReplWriteRequestListener requestListener)
		{
			_requestListener = requestListener;
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
			
			_requestListener.LoadFiles(filePaths);
		}

		public void ExplorerSelectionChanged(List<SolutionItem> selectedItems)
		{
			_selectedItems = selectedItems;
		}
	}
}
