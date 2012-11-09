using Clojure.Workspace.Repl.Presentation;

namespace Clojure.Workspace.Repl
{
	public class ReplCommandRouter : IRepl, IReplActivationListener
	{
		private IRepl _activeRepl;

		public void Write(string data)
		{
			_activeRepl.Write(data);
		}

		public void Submit(string expression)
		{
			_activeRepl.Submit(expression);
		}

		public void AddReplWriteCompleteListener(IReplWriteCompleteListener listener)
		{
			_activeRepl.AddReplWriteCompleteListener(listener);
		}

		public void AddReplOutputListener(IReplOutputListener listener)
		{
			_activeRepl.AddReplOutputListener(listener);
		}

		public void Start()
		{
			_activeRepl.Start();
		}

		public void Stop()
		{
			_activeRepl.Stop();
		}

		public void ReplActivated(IRepl repl)
		{
			_activeRepl = repl;
		}
	}
}
