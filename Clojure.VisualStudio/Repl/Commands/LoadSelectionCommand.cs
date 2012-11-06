using System;
using Clojure.Code.Repl;
using Clojure.VisualStudio.Repl.Presentation;
using Clojure.VisualStudio.Workspace;
using Clojure.VisualStudio.Workspace.TextEditor;

namespace Clojure.VisualStudio.Repl.Commands
{
	public class LoadSelectionCommand : IMenuCommandListener, ITextEditorStateChangeListener, IReplActivationListener
	{
		private TextEditorSnapshot _snapshot;
		private IRepl _repl;

		public void OnMenuCommandClick()
		{
			_repl.Write(_snapshot.Selection);
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
