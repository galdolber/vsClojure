using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE;
using EnvDTE80;

namespace Clojure.VisualStudio.SolutionExplorer
{
	public class VisualStudioExplorer : IExplorer
	{
		private readonly DTE2 _dte;
		private readonly SelectionEvents _selectionEvents;
		private readonly List<IExplorerSelectionChangedListener> _listeners;

		public VisualStudioExplorer(DTE2 dte)
		{
			_dte = dte;
			_selectionEvents = _dte.Events.SelectionEvents;
			_selectionEvents.OnChange += FileSelectionChanged;
			_listeners = new List<IExplorerSelectionChangedListener>();
		}

		public void AddSelectionListener(IExplorerSelectionChangedListener listener)
		{
			_listeners.Add(listener);
		}

		private void FileSelectionChanged()
		{
			var selectedItems = new List<SolutionItem>();

			for (int i = 0; i < _dte.SelectedItems.Count; i++)
			{
				var currentItem = _dte.SelectedItems.Item(i);

				if (currentItem.ProjectItem == null)
				{
					selectedItems.Add(new SolutionItem(currentItem.Project.FullName, SolutionItemType.Project));
				}
				else
				{
					selectedItems.Add(new SolutionItem(currentItem.ProjectItem.Name, SolutionItemType.File));
				}
			}

			_listeners.ForEach(l => l.ExplorerSelectionChanged(selectedItems));
		}

		public List<SolutionItem> FindProjectFiles(SolutionItem projectItem)
		{
			return new List<SolutionItem>();
		}
	}
}
