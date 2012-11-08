using System.Collections.Generic;

namespace Clojure.Workspace.Explorer
{
	public interface IExplorer
	{
		List<SolutionItem> FindProjectFiles(SolutionItem projectItem);
		void AddSelectionListener(IExplorerSelectionChangedListener listener);
	}
}