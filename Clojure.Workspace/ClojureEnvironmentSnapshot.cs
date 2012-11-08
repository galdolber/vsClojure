namespace Clojure.Workspace
{
	public class ClojureEnvironmentSnapshot
	{
		private readonly ClojureEnvironmentState _state;

		public ClojureEnvironmentSnapshot(ClojureEnvironmentState state)
		{
			_state = state;
		}

		public ClojureEnvironmentState State
		{
			get { return _state; }
		}

		public ClojureEnvironmentSnapshot ChangeState(ClojureEnvironmentState newState)
		{
			return new ClojureEnvironmentSnapshot(newState);
		}

		public static ClojureEnvironmentSnapshot Empty = new ClojureEnvironmentSnapshot(ClojureEnvironmentState.ReplAndEditorNotActive);
	}
}
