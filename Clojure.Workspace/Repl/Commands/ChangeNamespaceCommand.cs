using Clojure.Workspace.Menus;
using Clojure.Workspace.Repl.Presentation;
using Clojure.Workspace.TextEditor;

namespace Clojure.Workspace.Repl.Commands
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
