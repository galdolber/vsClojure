using Clojure.System.Collections;
using Clojure.Workspace.Menus;
using Clojure.Workspace.Repl.Presentation;
using Clojure.Workspace.TextEditor;

namespace Clojure.Workspace.Repl.Commands
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