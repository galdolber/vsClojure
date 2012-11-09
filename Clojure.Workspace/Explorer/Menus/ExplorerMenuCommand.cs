using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Clojure.Workspace.Menus;

namespace Clojure.Workspace.Explorer.Menus
{
	public class ExplorerMenuCommand : IExplorerSelectionChangedListener, IExternalClickListener
	{
		private readonly List<IExplorerMenuCommandListener> _listeners;
		private List<SolutionItem> _selectedItems;

		public ExplorerMenuCommand()
		{
			_listeners = new List<IExplorerMenuCommandListener>();
		}

		public void AddClickListener(IExplorerMenuCommandListener listener)
		{
			_listeners.Add(listener);
		}

		public void ExplorerSelectionChanged(List<SolutionItem> selectedItems)
		{
			_selectedItems = selectedItems;
		}

		public void OnExternalClick()
		{
			_listeners.ForEach(l => l.OnClick(_selectedItems));
		}
	}
}
