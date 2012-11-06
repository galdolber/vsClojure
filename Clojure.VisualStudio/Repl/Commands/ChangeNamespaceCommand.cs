using System;
using Clojure.Code.Repl;
using Clojure.VisualStudio.Repl.Presentation;
using Clojure.VisualStudio.Workspace;
using Clojure.VisualStudio.Workspace.TextEditor;

namespace Clojure.VisualStudio.Repl.Commands
{
	public class ChangeNamespaceCommand : IMenuCommandListener, ITextEditorStateChangeListener, IReplActivationListener
	{
		private TextEditorSnapshot _snapshot;
		private IRepl _repl;

		public void OnMenuCommandClick()
		{
			_repl.ChangeNamespace(_snapshot.Tokens);
		}

		public void OnTextEditorStateChange(TextEditorSnapshot snapshot)
		{
			_snapshot = snapshot;
		}

		public void ReplActivated(IRepl repl)
		{
			_repl = repl;
		}
	}
}
