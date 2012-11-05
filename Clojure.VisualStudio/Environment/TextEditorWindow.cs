using System.Collections.Generic;
using EnvDTE;
using EnvDTE80;

namespace Clojure.VisualStudio.Environment
{
	public class TextEditorWindow
	{
		private readonly WindowEvents _windowEvents;
		private readonly DTE2 _dte;
		private readonly List<ITextEditorDocumentChangedListener> _listeners;

		public TextEditorWindow(DTE2 dte)
		{
			_dte = dte;
			_windowEvents = dte.Events.WindowEvents;
			_windowEvents.WindowActivated += (o, e) => ActiveDocumentChanged();
			_listeners = new List<ITextEditorDocumentChangedListener>();
		}

		public void AddTextEditorDocumentChangedListener(ITextEditorDocumentChangedListener listener)
		{
			_listeners.Add(listener);
		}

		private void ActiveDocumentChanged()
		{
			_listeners.ForEach(l => l.OnTextEditorDocumentChange(_dte.ActiveDocument == null ? "" : _dte.ActiveDocument.FullName));
		}
	}

	public interface ITextEditorDocumentChangedListener
	{
		void OnTextEditorDocumentChange(string newDocumentPath);
	}
}
