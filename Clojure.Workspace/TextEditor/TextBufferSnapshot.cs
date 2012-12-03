using System.Collections.Generic;
using Clojure.Code.Parsing;

namespace Clojure.Workspace.TextEditor
{
	public class TextBufferSnapshot
	{
		private readonly LinkedList<Token> _tokens;
		private readonly string _selection;
		private readonly List<string> _selectedLines;
		private readonly string _filePath;
		private readonly int _caretPosition;

		public TextBufferSnapshot(LinkedList<Token> tokens, string selection, List<string> selectedLines, string filePath, int caretPosition)
		{
			_tokens = tokens;
			_caretPosition = caretPosition;
			_selectedLines = selectedLines;
			_filePath = filePath;
			_selection = selection;
		}

		public string FilePath
		{
			get { return _filePath; }
		}

		public string Selection
		{
			get { return _selection; }
		}

		public List<string> SelectedLines
		{
			get { return _selectedLines; }
		}

		public LinkedList<Token> Tokens
		{
			get { return _tokens; }
		}

		public int CaretPosition
		{
			get { return _caretPosition; }
		}

		public TextBufferSnapshot ChangeSelection(string newSelection)
		{
			return new TextBufferSnapshot(_tokens, newSelection, _selectedLines, _filePath, _caretPosition);
		}

		public TextBufferSnapshot ChangeTokens(LinkedList<Token> newTokens)
		{
			return new TextBufferSnapshot(newTokens, _selection, _selectedLines, _filePath, _caretPosition);
		}

		public TextBufferSnapshot ChangeSelectedLines(List<string> newSelectedLines)
		{
			return new TextBufferSnapshot(_tokens, _selection, newSelectedLines, _filePath, _caretPosition);
		}

		public TextBufferSnapshot ChangeFilePath(string newPath)
		{
			return new TextBufferSnapshot(_tokens, _selection, _selectedLines, newPath, _caretPosition);
		}

		public TextBufferSnapshot ChangeCaretPosition(int newCaretPosition)
		{
			return new TextBufferSnapshot(_tokens, _selection, _selectedLines, _filePath, newCaretPosition);
		}

		public static TextBufferSnapshot Empty()
		{
			return new TextBufferSnapshot(new LinkedList<Token>(), "", new List<string>(), "", 0);
		}
	}
}