using System.Collections.Generic;
using Clojure.Workspace.TextEditor;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Clojure.VisualStudio.Workspace.TextEditor
{
	public class ClojureEditorCollection
	{
		private readonly WindowEvents _windowEvents;
		private readonly DTE2 _dte;
		private readonly List<IActiveEditorChangeListener> _listeners;
		private readonly IVsEditorAdaptersFactoryService _vsEditorAdaptersFactoryService;
		private readonly IVsTextManager _vsTextManager;

		public static Dictionary<ITextView, VisualStudioClojureTextEditor> Editors = new Dictionary<ITextView, VisualStudioClojureTextEditor>();

		public ClojureEditorCollection(DTE2 dte, IVsEditorAdaptersFactoryService vsEditorAdaptersFactoryService, IVsTextManager vsTextManager)
		{
			_dte = dte;
			_vsTextManager = vsTextManager;
			_vsEditorAdaptersFactoryService = vsEditorAdaptersFactoryService;
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

			IVsTextView activeView = null;
			_vsTextManager.GetActiveView(0, null, out activeView);
			ITextView textView = _vsEditorAdaptersFactoryService.GetWpfTextView(activeView);
			_listeners.ForEach(l => l.OnActiveEditorChange(Editors[textView]));
		}
	}
}