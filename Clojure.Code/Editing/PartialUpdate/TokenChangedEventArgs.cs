using System;
using Clojure.Code.Parsing;

namespace Clojure.Code.Editing.PartialUpdate
{
	public class TokenChangedEventArgs : EventArgs
	{
		private readonly IndexToken _indexToken;

		public TokenChangedEventArgs(IndexToken indexToken)
		{
			_indexToken = indexToken;
		}

		public IndexToken IndexToken
		{
			get { return _indexToken; }
		}
	}
}