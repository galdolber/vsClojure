using System.Collections.Generic;
using Clojure.Code.Parsing;
using Clojure.System.State;
using Microsoft.VisualStudio.Text;

namespace Clojure.VisualStudio.Editor
{
	public class TokenizedBufferBuilder
	{
		private readonly Tokenizer _tokenizer;
		public static Dictionary<ITextBuffer, Entity<LinkedList<Token>>> TokenizedBuffers = new Dictionary<ITextBuffer, Entity<LinkedList<Token>>>();

		public TokenizedBufferBuilder(Tokenizer tokenizer)
		{
			_tokenizer = tokenizer;
		}

		public void CreateTokenizedBuffer(ITextBuffer buffer)
		{
			Entity<LinkedList<Token>> tokenizedBuffer = new Entity<LinkedList<Token>>();
			tokenizedBuffer.CurrentState = _tokenizer.Tokenize(buffer.CurrentSnapshot.GetText());
			TokenizedBuffers.Add(buffer, tokenizedBuffer);
		}

		public void RemoveTokenizedBuffer(ITextBuffer buffer)
		{
			if (TokenizedBuffers.ContainsKey(buffer)) TokenizedBuffers.Remove(buffer);
		}
	}
}