using Clojure.System.CommandWindow;

namespace Clojure.Workspace.Repl
{
	public interface IRepl : IReplWriteRequestListener, ISubmitCommandListener
	{
		void AddReplWriteCompleteListener(IReplWriteCompleteListener listener);
		void AddReplOutputListener(IReplOutputListener listener);
		void Start();
		void Stop();
	}
}