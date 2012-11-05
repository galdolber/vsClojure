using System;
using System.Collections.Generic;
using System.Windows.Controls;
using EnvDTE;

namespace Clojure.VisualStudio.Environment
{
	public class ClojureEnvironment : ITextEditorDocumentChangedListener
	{
		private readonly List<IEnvironmentListener> _listeners;
		private ClojureEnvironmentSnapshot _snapshot;
		private readonly TabItem _replTab;

		public ClojureEnvironment(TabItem replTab)
		{
			_replTab = replTab;
			_listeners = new List<IEnvironmentListener>();
			_snapshot = new ClojureEnvironmentSnapshot(ClojureEnvironmentState.ReplAndEditorNotActive, "");
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
			if (_replTab.IsSelected) HandleReplActivated();
			else HandleReplDeactivated();
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

		public void OnTextEditorDocumentChange(string newDocumentPath)
		{
			_snapshot = _snapshot.ChangeActiveDocumentPath(newDocumentPath);
			if (!newDocumentPath.ToLower().EndsWith(".clj")) HandleNonClojureDocument();
			else HandleClojureDocument();
		}
	}
}