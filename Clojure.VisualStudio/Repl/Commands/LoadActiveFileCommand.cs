using System;
using Clojure.Code.Repl;
using Clojure.System.Collections;
using Clojure.VisualStudio.Repl.Presentation;
using Clojure.VisualStudio.Workspace;
using Clojure.VisualStudio.Workspace.EditorWindow;

namespace Clojure.VisualStudio.Repl.Commands
{
	public class LoadActiveFileCommand : IMenuCommandListener, ITextEditorWindowActiveDocumentChangedListener, IReplActivationListener
	{
		private string _activeDocumentPath;
		private IRepl _repl;

		public void OnMenuCommandClick()
		{
			_repl.LoadFiles(_activeDocumentPath.SingletonAsList());
		}

		public void OnActiveDocumentChange(string newDocumentPath)
		{
			_activeDocumentPath = newDocumentPath;
		}

		public void ReplActivated(IRepl repl)
		{
			_repl = repl;
		}
	}
}