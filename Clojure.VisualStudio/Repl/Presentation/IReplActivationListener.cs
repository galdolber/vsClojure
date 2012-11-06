using Clojure.Code.Repl;

namespace Clojure.VisualStudio.Repl.Presentation
{
	public interface IReplActivationListener
	{
		void ReplActivated(IRepl repl);
	}
}
