using System;
using System.Collections.Generic;
using Clojure.Code.Editing.Formatting;
using Clojure.Workspace.TextEditor.Options;

namespace Clojure.Workspace.TextEditor.Commands
{
	public class AutoFormatCommand : IEditorMenuCommandListener, IEditorOptionsChangedListener
	{
		private EditorOptions _currentOptions;
		private readonly ITextEditorCommandListener _textEditor;

		public AutoFormatCommand(ITextEditorCommandListener textEditor)
		{
			_textEditor = textEditor;
		}

		public void OnOptionChange(EditorOptions newOptions)
		{
			_currentOptions = newOptions;
		}

		public void Selected(TextEditorSnapshot snapshot)
		{
			var autoFormatter = new AutoFormat();
			var formattedBuffer = autoFormatter.Format(snapshot.Tokens, _currentOptions.IndentSize);
			_textEditor.OnAutoFormat(formattedBuffer);
		}
	}
}