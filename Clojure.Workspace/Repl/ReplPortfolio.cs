using System.Collections.Generic;

namespace Clojure.Workspace.Repl
{
	public interface IReplCollector
	{
		void AddRepl(IRepl repl);
	}

	public class ReplPortfolio : IReplCollector
	{
		private readonly List<IReplPortfolioListener> _listeners;

		public ReplPortfolio()
		{
			_listeners = new List<IReplPortfolioListener>();
		}

		public void AddPortfolioListener(IReplPortfolioListener listener)
		{
			_listeners.Add(listener);
		}

		public void AddRepl(IRepl repl)
		{
			_listeners.ForEach(l => l.ReplAdded(repl));
		}
	}

	public interface IReplPortfolioListener
	{
		void ReplAdded(IRepl repl);
	}
}