using Clojure.Code.Repl;
using Clojure.System.Collections;
using Clojure.VisualStudio.Workspace;
using Clojure.VisualStudio.Workspace.EditorWindow;

namespace Clojure.VisualStudio.Repl.Commands
{
	public class LoadActiveFileCommand : IMenuCommandListener, ITextEditorWindowActiveDocumentChangedListener
	{
		private string _activeDocumentPath;
		private readonly IReplWriteRequestListener _replWriteRequestListener;

		public LoadActiveFileCommand(IReplWriteRequestListener replWriteRequestListener)
		{
			_replWriteRequestListener = replWriteRequestListener;
		}

		public void OnMenuCommandClick()
		{
			_replWriteRequestListener.LoadFiles(_activeDocumentPath.SingletonAsList());
		}

		public void OnActiveDocumentChange(string newDocumentPath)
		{
			_activeDocumentPath = newDocumentPath;
		}
	}
}