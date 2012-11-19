using System.Collections.Generic;
using Clojure.Workspace.Menus;

namespace Clojure.Workspace.TextEditor
{
	public class MenuCommandCollection : IEnvironmentListener
	{
		public static readonly List<ClojureEnvironmentState> VisibleEditorStates = new List<ClojureEnvironmentState>()
			{
				ClojureEnvironmentState.EditorActiveOnly,
				ClojureEnvironmentState.ReplAndEditorActive
			};

		public static readonly List<ClojureEnvironmentState> VisibleReplStates = new List<ClojureEnvironmentState>()
			{
				ClojureEnvironmentState.ReplAndEditorActive
			};

		private ClojureEnvironmentSnapshot _lastKnownEnvironmentState;
		private readonly List<IMenuCommand> _menuCommands;
		private readonly List<ClojureEnvironmentState> _visibleStates;

		public MenuCommandCollection(List<ClojureEnvironmentState> visibleStates)
		{
			_visibleStates = visibleStates;
			_menuCommands = new List<IMenuCommand>();
			_lastKnownEnvironmentState = new ClojureEnvironmentSnapshot(ClojureEnvironmentState.Unknown);
		}

		public void Add(IMenuCommand menuCommand)
		{
			_menuCommands.Add(menuCommand);
		}

		public void EnvironmentStateChange(ClojureEnvironmentSnapshot snapshot)
		{
			bool wasVisible = _visibleStates.Contains(_lastKnownEnvironmentState.State);
			bool wasInvisible = !wasVisible;
			_lastKnownEnvironmentState = snapshot;

			if (wasInvisible && _visibleStates.Contains(snapshot.State)) _menuCommands.ForEach(m => m.Show());
			else if (wasVisible && !_visibleStates.Contains(snapshot.State)) _menuCommands.ForEach(l => l.Hide());
		}
	}
}