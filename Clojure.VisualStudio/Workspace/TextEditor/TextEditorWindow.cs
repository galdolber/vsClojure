using System.Collections.Generic;
using Clojure.Workspace.TextEditor;
using EnvDTE;
using EnvDTE80;

namespace Clojure.VisualStudio.Workspace.TextEditor
{
	public class TextEditorWindow
	{
		private readonly WindowEvents _windowEvents;
		private readonly DTE2 _dte;
		private readonly List<IActiveEditorChangeListener> _listeners;

		public TextEditorWindow(DTE2 dte)
		{
			_dte = dte;
			_windowEvents = dte.Events.WindowEvents;
			_windowEvents.WindowActivated += (o, e) => ActiveDocumentChanged();
			_listeners = new List<IActiveEditorChangeListener>();
		}

		public void AddEditorChangeListener(IActiveEditorChangeListener listener)
		{
			_listeners.Add(listener);
		}

		private void ActiveDocumentChanged()
		{
			var activeEditorPath = _dte.ActiveDocument == null ? "" : _dte.ActiveDocument.FullName;
			if (!activeEditorPath.ToLower().EndsWith(".clj")) return;

			_listeners.ForEach(l => l.OnActiveDocumentChange(_dte.ActiveDocument == null ? "" : _dte.ActiveDocument.FullName));
		}
	}
}