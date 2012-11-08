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
		private readonly List<ITextEditorWindowActiveDocumentChangedListener> _listeners;

		public TextEditorWindow(DTE2 dte)
		{
			_dte = dte;
			_windowEvents = dte.Events.WindowEvents;
			_windowEvents.WindowActivated += (o, e) => ActiveDocumentChanged();
			_listeners = new List<ITextEditorWindowActiveDocumentChangedListener>();
		}

		public void AddActiveDocumentChangedListener(ITextEditorWindowActiveDocumentChangedListener listener)
		{
			_listeners.Add(listener);
		}

		private void ActiveDocumentChanged()
		{
			_listeners.ForEach(l => l.OnActiveDocumentChange(_dte.ActiveDocument == null ? "" : _dte.ActiveDocument.FullName));
		}
	}
}