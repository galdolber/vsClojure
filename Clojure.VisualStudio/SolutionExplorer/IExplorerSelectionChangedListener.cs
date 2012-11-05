using System.Collections.Generic;

namespace Clojure.VisualStudio.SolutionExplorer
{
	public interface IExplorerSelectionChangedListener
	{
		void ExplorerSelectionChanged(List<SolutionItem> selectedItems);
	}
}