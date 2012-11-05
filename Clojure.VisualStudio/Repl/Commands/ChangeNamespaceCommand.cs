using Clojure.Code.Repl;
using Clojure.VisualStudio.Workspace;
using Clojure.VisualStudio.Workspace.TextEditor;

namespace Clojure.VisualStudio.Repl.Commands
{
	public class ChangeNamespaceCommand : IMenuCommandListener, ITextEditorStateChangeListener
	{
		private readonly IReplWriteRequestListener _replWriteRequestListener;
		private TextEditorSnapshot _snapshot;

		public ChangeNamespaceCommand(IReplWriteRequestListener replWriteRequestListener)
		{
			_replWriteRequestListener = replWriteRequestListener;
		}

		public void OnMenuCommandClick()
		{
			_replWriteRequestListener.ChangeNamespace(_snapshot.Tokens);
		}

		public void OnTextEditorStateChange(TextEditorSnapshot snapshot)
		{
			_snapshot = snapshot;
		}
	}
}
