using System.Collections.Generic;
using Clojure.Code.Parsing;

namespace Clojure.Workspace.TextEditor
{
	public class BufferDiffGram
	{
		private readonly List<Token> _oldTokens;
		private readonly List<IndexToken> _newTokens;

		public BufferDiffGram(List<Token> oldTokens, List<IndexToken> newTokens)
		{
			_oldTokens = oldTokens;
			_newTokens = newTokens;
		}

		public List<IndexToken> NewTokens
		{
			get { return _newTokens; }
		}

		public List<Token> OldTokens
		{
			get { return _oldTokens; }
		}
	}
}