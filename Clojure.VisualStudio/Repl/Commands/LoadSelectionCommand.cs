using Clojure.Code.Repl;
using Clojure.VisualStudio.Workspace;
using Clojure.VisualStudio.Workspace.TextEditor;

namespace Clojure.VisualStudio.Repl.Commands
{
	public class LoadSelectionCommand : IMenuCommandListener, ITextEditorStateChangeListener
	{
		private readonly IReplWriteRequestListener _replWriteRequestListener;
		private TextEditorSnapshot _snapshot;

		public LoadSelectionCommand(IReplWriteRequestListener replWriteRequestListener)
		{
			_replWriteRequestListener = replWriteRequestListener;
		}

		public void OnMenuCommandClick()
		{
			_replWriteRequestListener.Write(_snapshot.Selection);
		}

		public void OnTextEditorStateChange(TextEditorSnapshot snapshot)
		{
			_snapshot = snapshot;
		}
	}
}
