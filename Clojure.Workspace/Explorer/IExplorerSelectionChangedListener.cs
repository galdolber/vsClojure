using System.Collections.Generic;

namespace Clojure.Workspace.Explorer
{
	public interface IExplorerSelectionChangedListener
	{
		void ExplorerSelectionChanged(List<SolutionItem> selectedItems);
	}
}