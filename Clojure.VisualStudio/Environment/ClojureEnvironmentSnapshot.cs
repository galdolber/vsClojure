using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clojure.VisualStudio.Environment
{
	public class ClojureEnvironmentSnapshot
	{
		private readonly ClojureEnvironmentState _state;
		private readonly string _activeDocumentPath;

		public ClojureEnvironmentSnapshot(ClojureEnvironmentState state, string activeDocumentPath)
		{
			_activeDocumentPath = activeDocumentPath;
			_state = state;
		}

		public string ActiveDocumentPath
		{
			get { return _activeDocumentPath; }
		}

		public ClojureEnvironmentState State
		{
			get { return _state; }
		}

		public ClojureEnvironmentSnapshot ChangeState(ClojureEnvironmentState newState)
		{
			return new ClojureEnvironmentSnapshot(newState, _activeDocumentPath);
		}

		public ClojureEnvironmentSnapshot ChangeActiveDocumentPath(string newDocumentPath)
		{
			return new ClojureEnvironmentSnapshot(_state, newDocumentPath);
		}
	}
}
