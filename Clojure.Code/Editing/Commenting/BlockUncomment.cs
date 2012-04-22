using System.Collections.Generic;
using System.IO;
using Clojure.Code.Parsing;

namespace Clojure.Code.Editing.Commenting
{
	public class BlockUncomment
	{
		public List<string> Execute(List<string> lines)
		{
			var uncommentedLines = new List<string>();

			foreach (var line in lines)
			{
				var lexer = new Lexer(new PushBackCharacterStream(new StringReader(line)));
				var currentToken = lexer.Next();
				while (currentToken != null && currentToken.Type == TokenType.Whitespace) currentToken = lexer.Next();
				if (currentToken == null) uncommentedLines.Add(line);
				else if (currentToken.Type != TokenType.Comment) uncommentedLines.Add(line);
				else uncommentedLines.Add(line.Remove(currentToken.StartIndex, 1));
			}

			return uncommentedLines;
		}
	}
}