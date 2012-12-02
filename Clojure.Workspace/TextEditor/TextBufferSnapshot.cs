using System;
using System.Collections.Generic;
using Clojure.Code.Parsing;

namespace Clojure.Workspace.TextEditor
{
	// Rename this to buffer snapshot.
	public class TextBufferSnapshot
	{
		private readonly LinkedList<Token> _tokens;
		private readonly string _selection;
		private readonly List<string> _selectedLines;
		private readonly string _filePath;

		public TextBufferSnapshot(LinkedList<Token> tokens, string selection, List<string> selectedLines, string filePath)
		{
			_tokens = tokens;
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

		public TextBufferSnapshot ChangeSelection(string newSelection)
		{
			return new TextBufferSnapshot(_tokens, newSelection, _selectedLines, _filePath);
		}

		public TextBufferSnapshot ChangeTokens(LinkedList<Token> newTokens)
		{
			return new TextBufferSnapshot(newTokens, _selection, _selectedLines, _filePath);
		}

		public TextBufferSnapshot ChangeSelectedLines(List<string> newSelectedLines)
		{
			return new TextBufferSnapshot(_tokens, _selection, newSelectedLines, _filePath);
		}

		public TextBufferSnapshot ChangeFilePath(string newPath)
		{
			return new TextBufferSnapshot(_tokens, _selection, _selectedLines, newPath);
		}

		public static TextBufferSnapshot Empty = new TextBufferSnapshot(new LinkedList<Token>(), "", new List<string>(), "");
	}
}