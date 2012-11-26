using System.Collections.Generic;
using System.IO;
using System.Linq;
using Clojure.Code.Editing.PartialUpdate;
using Clojure.Code.Parsing;

namespace Clojure.Workspace.TextEditor
{
	public static class TextChangeExtensions
	{
		public static BufferDiffGram ApplyChange(this LinkedList<Token> tokenList, TextChangeData change, string snapshot)
		{
			if (tokenList.Count == 0) return tokenList.AddNewTokensToBuffer(snapshot);
			if (snapshot.Length == 0) return tokenList.RemoveAllTokens();

			var firstToken = tokenList.FindTokenBeforeIndex(change.Position);
			var lexer = new Lexer(new PushBackCharacterStream(new StringReader(snapshot.Substring(firstToken.IndexToken.StartIndex))));
			var oldBufferStartIndex = firstToken.IndexToken.StartIndex + change.Delta;
			var newBufferStartIndex = firstToken.IndexToken.StartIndex;
			var endPosition = change.Position + change.LengthOfChangedText;
			var oldTokens = new LinkedList<LinkedListNode<Token>>();
			var newTokens = new LinkedList<IndexToken>();
			var newToken = lexer.Next();
			var oldToken = firstToken;

			while (newBufferStartIndex + newToken.Length != oldBufferStartIndex + oldToken.IndexToken.Token.Length
			       || (change.Delta < 0 && oldToken.IndexToken.StartIndex + oldToken.IndexToken.Token.Length < endPosition)
			       || (change.Delta > 0 && newBufferStartIndex + newToken.Length < endPosition))
			{
				if (newBufferStartIndex + newToken.Length < oldBufferStartIndex + oldToken.IndexToken.Token.Length)
				{
					newTokens.AddLast(new IndexToken(newBufferStartIndex, newToken));
					newBufferStartIndex += newToken.Length;
					newToken = lexer.Next();
				}
				else
				{
					oldTokens.AddLast(oldToken.Node);
					oldBufferStartIndex += oldToken.IndexToken.Token.Length;
					oldToken = oldToken.Next();
				}
			}

			oldTokens.AddLast(oldToken.Node);
			newTokens.AddLast(new IndexToken(newBufferStartIndex, newToken));
			foreach (var t in newTokens) tokenList.AddBefore(firstToken.Node, t.Token);
			foreach (var t in oldTokens) tokenList.Remove(t);

			return new BufferDiffGram(
				oldTokens.Select(t => t.Value).ToList(), newTokens.ToList());
		}

		private static BufferDiffGram RemoveAllTokens(this LinkedList<Token> tokenList)
		{
			var diffGram = new BufferDiffGram(tokenList.ToList(), new List<IndexToken>());
			tokenList.Clear();
			return diffGram;
		}

		private static BufferDiffGram AddNewTokensToBuffer(this LinkedList<Token> tokenList, string text)
		{
			var lexer = new Lexer(new PushBackCharacterStream(new StringReader(text)));
			var currentToken = lexer.Next();
			var newTokens = new LinkedList<IndexToken>();
			var currentIndex = 0;

			while (currentToken != null)
			{
				tokenList.AddLast(currentToken);
				newTokens.AddLast(new IndexToken(currentIndex, currentToken));
				currentIndex += currentToken.Length;
				currentToken = lexer.Next();
			}

			return new BufferDiffGram(new List<Token>(), newTokens.ToList());
		}
	}
}