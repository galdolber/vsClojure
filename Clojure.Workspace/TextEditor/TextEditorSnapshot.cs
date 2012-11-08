using System.Collections.Generic;
using Clojure.Code.Parsing;

namespace Clojure.Workspace.TextEditor
{
	public class TextEditorSnapshot
	{
		private readonly LinkedList<Token> _tokens;
		private readonly string _selection;

		public TextEditorSnapshot(LinkedList<Token> tokens, string selection)
		{
			_tokens = tokens;
			_selection = selection;
		}

		public string Selection
		{
			get { return _selection; }
		}

		public LinkedList<Token> Tokens
		{
			get { return _tokens; }
		}

		public TextEditorSnapshot ChangeSelection(string newSelection)
		{
			return new TextEditorSnapshot(_tokens, newSelection);
		}

		public TextEditorSnapshot ChangeTokens(LinkedList<Token> newTokens)
		{
			return new TextEditorSnapshot(newTokens, _selection);
		}

		public static TextEditorSnapshot Empty = new TextEditorSnapshot(new LinkedList<Token>(), "");
	}
}