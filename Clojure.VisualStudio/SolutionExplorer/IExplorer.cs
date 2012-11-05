using System.Collections.Generic;

namespace Clojure.VisualStudio.SolutionExplorer
{
	public interface IExplorer
	{
		List<SolutionItem> FindProjectFiles(SolutionItem projectItem);
		void AddSelectionListener(IExplorerSelectionChangedListener listener);
	}
}