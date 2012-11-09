using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clojure.Workspace.Explorer.Menus
{
	public interface IExplorerMenuCommandListener
	{
		void OnClick(List<SolutionItem> selectedItems);
	}
}
