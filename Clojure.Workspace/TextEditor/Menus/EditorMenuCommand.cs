using System;
using System.Collections.Generic;
using Clojure.Workspace.TextEditor;

namespace Clojure.Workspace.Menus
{
	public class EditorMenuCommand : ITextEditorStateChangeListener, IExternalClickListener
	{
		private readonly List<IEditorMenuCommandListener> _listeners;
		private TextEditorSnapshot _snapshot;

		public EditorMenuCommand()
		{
			_listeners = new List<IEditorMenuCommandListener>();
		}

		public void AddMenuCommandListener(IEditorMenuCommandListener listener)
		{
			_listeners.Add(listener);
		}

		public void OnTextEditorStateChange(TextEditorSnapshot snapshot)
		{
			_snapshot = snapshot;
		}

		public void OnExternalClick()
		{
			_listeners.ForEach(l => l.Selected(_snapshot));
		}
	}
}