using Clojure.Code.Parsing;

namespace Clojure.Code.Editing.BraceMatching
{
	public class MatchingBracePair
	{
		private readonly IndexToken _start;
		private readonly IndexToken _end;

		public MatchingBracePair(IndexToken start, IndexToken end)
		{
			_start = start;
			_end = end;
		}

		public IndexToken End
		{
			get { return _end; }
		}

		public IndexToken Start
		{
			get { return _start; }
		}
	}
}