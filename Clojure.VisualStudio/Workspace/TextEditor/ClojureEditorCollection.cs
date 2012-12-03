using System.Collections.Generic;
using EnvDTE;
using EnvDTE80;

namespace Clojure.VisualStudio.Workspace.TextEditor
{
	public class ClojureEditorCollection
	{
		private readonly WindowEvents _windowEvents;
		private readonly DTE2 _dte;
		private readonly List<IActiveEditorChangeListener> _listeners;
		private readonly Dictionary<string, VisualStudioClojureTextView> _editors = new Dictionary<string, VisualStudioClojureTextView>();

		public ClojureEditorCollection(DTE2 dte)
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

		public void EditorAdded(string path, VisualStudioClojureTextView editor)
		{
			_editors.Add(path, editor);
		}

		private void ActiveDocumentChanged()
		{
			var activeEditorPath = _dte.ActiveDocument == null ? "" : _dte.ActiveDocument.FullName;
			if (!activeEditorPath.ToLower().EndsWith(".clj")) _listeners.ForEach(l => l.NonClojureEditorActivated());
			else _listeners.ForEach(l => l.OnActiveEditorChange(_editors[activeEditorPath]));
		}
	}
}