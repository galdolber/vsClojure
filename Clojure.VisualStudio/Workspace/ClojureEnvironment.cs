using System.Collections.Generic;
using System.Windows.Controls;
using Clojure.VisualStudio.Workspace.EditorWindow;

namespace Clojure.VisualStudio.Workspace
{
	public class ClojureEnvironment : ITextEditorWindowActiveDocumentChangedListener
	{
		private readonly List<IEnvironmentListener> _listeners;
		private ClojureEnvironmentSnapshot _snapshot;

		public ClojureEnvironment()
		{
			_listeners = new List<IEnvironmentListener>();
			_snapshot = ClojureEnvironmentSnapshot.Empty;
		}

		public void AddActivationListener(IEnvironmentListener listener)
		{
			_listeners.Add(listener);
		}

		private void HandleClojureDocument()
		{
			if (_snapshot.State == ClojureEnvironmentState.ReplActiveOnly)
			{
				_snapshot = _snapshot.ChangeState(ClojureEnvironmentState.ReplAndEditorActive);
				FireStateChangeEvent();
			}
			else if (_snapshot.State == ClojureEnvironmentState.ReplAndEditorNotActive)
			{
				_snapshot = _snapshot.ChangeState(ClojureEnvironmentState.EditorActiveOnly);
				FireStateChangeEvent();
			}
		}

		private void HandleNonClojureDocument()
		{
			if (_snapshot.State == ClojureEnvironmentState.ReplAndEditorActive)
			{
				_snapshot = _snapshot.ChangeState(ClojureEnvironmentState.ReplActiveOnly);
				FireStateChangeEvent();
			}
			else if (_snapshot.State == ClojureEnvironmentState.EditorActiveOnly)
			{
				_snapshot = _snapshot.ChangeState(ClojureEnvironmentState.ReplAndEditorNotActive);
				FireStateChangeEvent();
			}
		}

		private void FireStateChangeEvent()
		{
			_listeners.ForEach(l => l.EnvironmentStateChange(_snapshot));
		}

		public void OnReplActivated()
		{
			// Not sure what to do here.  Isn't a REPL always active?  We're creating the control right away for it.
		}

		private void HandleReplDeactivated()
		{
			if (_snapshot.State == ClojureEnvironmentState.ReplActiveOnly)
			{
				_snapshot = _snapshot.ChangeState(ClojureEnvironmentState.ReplAndEditorNotActive);
				FireStateChangeEvent();
			}
			else if (_snapshot.State == ClojureEnvironmentState.ReplAndEditorActive)
			{
				_snapshot = _snapshot.ChangeState(ClojureEnvironmentState.EditorActiveOnly);
				FireStateChangeEvent();
			}
		}

		private void HandleReplActivated()
		{
			if (_snapshot.State == ClojureEnvironmentState.ReplAndEditorNotActive)
			{
				_snapshot = _snapshot.ChangeState(ClojureEnvironmentState.ReplActiveOnly);
				FireStateChangeEvent();
			}
			else if (_snapshot.State == ClojureEnvironmentState.EditorActiveOnly)
			{
				_snapshot = _snapshot.ChangeState(ClojureEnvironmentState.ReplAndEditorActive);
				FireStateChangeEvent();
			}
		}

		public void OnActiveDocumentChange(string newDocumentPath)
		{
			if (!newDocumentPath.ToLower().EndsWith(".clj")) HandleNonClojureDocument();
			else HandleClojureDocument();
		}
	}
}