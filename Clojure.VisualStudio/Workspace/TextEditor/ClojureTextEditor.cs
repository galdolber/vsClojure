using System;
using System.Collections.Generic;
using Clojure.VisualStudio.Editor;
using Clojure.VisualStudio.Workspace.EditorWindow;
using Clojure.VisualStudio.Workspace.TextEditor.Commands;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Clojure.VisualStudio.Workspace.TextEditor
{
	public class ClojureTextEditor : ITextEditorWindowActiveDocumentChangedListener, IAutoFormatListener
	{
		private readonly IVsEditorAdaptersFactoryService _vsEditorAdaptersFactoryService;
		private readonly IVsTextManager _vsTextManager;
		private readonly List<ITextEditorStateChangeListener> _listeners;
		private ITextBuffer _currentBuffer;
		private string _currentDocumentPath;
		private IWpfTextView _currentWpfTextView;
		private TextEditorSnapshot _snapshot;

		public ClojureTextEditor(IVsEditorAdaptersFactoryService vsEditorAdaptersFactoryService, IVsTextManager vsTextManager)
		{
			_vsEditorAdaptersFactoryService = vsEditorAdaptersFactoryService;
			_vsTextManager = vsTextManager;
			_listeners = new List<ITextEditorStateChangeListener>();
			_snapshot = TextEditorSnapshot.Empty;
			_currentDocumentPath = "";
		}

		public void AddStateChangeListener(ITextEditorStateChangeListener listener)
		{
			_listeners.Add(listener);
		}

		public void OnActiveDocumentChange(string newDocumentPath)
		{
			if (_currentDocumentPath.ToLower().EndsWith(".clj"))
			{
				_currentBuffer.Changed -= DocumentEdited;
				_currentWpfTextView.Selection.SelectionChanged -= SelectionChanged;
			}

			if (newDocumentPath.ToLower().EndsWith(".clj"))
			{
				IVsTextView activeView = null;
				_vsTextManager.GetActiveView(0, null, out activeView);
				_currentWpfTextView = _vsEditorAdaptersFactoryService.GetWpfTextView(activeView);
				_currentBuffer = _currentWpfTextView.TextBuffer;
				_currentBuffer.Changed += DocumentEdited;
				_currentWpfTextView.Selection.SelectionChanged += SelectionChanged;
			}

			_currentDocumentPath = newDocumentPath;
		}

		private void SelectionChanged(object sender, EventArgs e)
		{
			_snapshot = _snapshot.ChangeSelection(_currentWpfTextView.Selection.StreamSelectionSpan.Snapshot.GetText());
			FireStateChangeEvent();
		}

		private void DocumentEdited(object sender, TextContentChangedEventArgs args)
		{
			var bufferState = TokenizedBufferBuilder.TokenizedBuffers[_currentBuffer].CurrentState;
			_snapshot = _snapshot.ChangeTokens(bufferState);
			FireStateChangeEvent();
		}

		private void FireStateChangeEvent()
		{
			_listeners.ForEach(l => l.OnTextEditorStateChange(_snapshot));
		}

		public void OnAutoFormat(string text)
		{
			_currentBuffer.Replace(new Span(0, _currentBuffer.CurrentSnapshot.Length), text);
		}
	}
}