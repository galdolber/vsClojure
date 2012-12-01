using System;
using System.Collections.Generic;
using Clojure.Code.Parsing;

namespace Clojure.Workspace.TextEditor
{
	// Rename this to buffer snapshot.
	public class TextEditorSnapshot
	{
		private readonly LinkedList<Token> _tokens;
		private readonly string _selection;
		private readonly List<string> _selectedLines;
		private readonly string _filePath;

		public TextEditorSnapshot(LinkedList<Token> tokens, string selection, List<string> selectedLines, string filePath)
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

		public TextEditorSnapshot ChangeSelection(string newSelection)
		{
			return new TextEditorSnapshot(_tokens, newSelection, _selectedLines, _filePath);
		}

		public TextEditorSnapshot ChangeTokens(LinkedList<Token> newTokens)
		{
			return new TextEditorSnapshot(newTokens, _selection, _selectedLines, _filePath);
		}

		public TextEditorSnapshot ChangeSelectedLines(List<string> newSelectedLines)
		{
			return new TextEditorSnapshot(_tokens, _selection, newSelectedLines, _filePath);
		}

		public TextEditorSnapshot ChangeFilePath(string newPath)
		{
			return new TextEditorSnapshot(_tokens, _selection, _selectedLines, newPath);
		}

		public static TextEditorSnapshot Empty = new TextEditorSnapshot(new LinkedList<Token>(), "", new List<string>(), "");
	}
}