using Clojure.Code.Repl;
using Clojure.System.Collections;
using Clojure.VisualStudio.Environment;

namespace Clojure.VisualStudio.Repl.Commands
{
	public class LoadActiveFileCommand : IMenuCommandListener, ITextEditorDocumentChangedListener
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

		public void OnTextEditorDocumentChange(string newDocumentPath)
		{
			_activeDocumentPath = newDocumentPath;
		}
	}
}