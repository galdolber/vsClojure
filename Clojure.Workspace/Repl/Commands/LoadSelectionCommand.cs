using Clojure.Workspace.Menus;
using Clojure.Workspace.Repl.Presentation;
using Clojure.Workspace.TextEditor;

namespace Clojure.Workspace.Repl.Commands
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
