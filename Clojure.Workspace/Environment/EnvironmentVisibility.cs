using System.Collections.Generic;

namespace Clojure.Workspace.TextEditor
{
	public class EnvironmentVisibility : IEnvironmentListener
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
		private readonly List<IVisibilityListener> _listeners;
		private readonly List<ClojureEnvironmentState> _visibleStates;

		public EnvironmentVisibility(List<ClojureEnvironmentState> visibleStates)
		{
			_visibleStates = visibleStates;
			_listeners = new List<IVisibilityListener>();
		}

		public void AddVisibilityListener(IVisibilityListener listener)
		{
			_listeners.Add(listener);
		}

		public void EnvironmentStateChange(ClojureEnvironmentSnapshot snapshot)
		{
			bool wasVisible = _visibleStates.Contains(_lastKnownEnvironmentState.State);
			bool wasInvisible = !wasVisible;
			_lastKnownEnvironmentState = snapshot;

			if (wasInvisible && _visibleStates.Contains(snapshot.State)) _listeners.ForEach(l => l.OnVisible());
			else if (wasVisible && !_visibleStates.Contains(snapshot.State)) _listeners.ForEach(l => l.OnInvisible());
		}
	}
}