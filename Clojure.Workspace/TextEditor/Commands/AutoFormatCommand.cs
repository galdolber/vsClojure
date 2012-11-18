using System;
using System.Collections.Generic;
using Clojure.Code.Editing.Formatting;
using Clojure.Workspace.Menus;
using Clojure.Workspace.TextEditor.Options;

namespace Clojure.Workspace.TextEditor.Commands
{
	public class AutoFormatCommand : IEditorOptionsChangedListener, ITextEditorStateChangeListener, IExternalClickListener
	{
		private EditorOptions _currentOptions;
		private readonly ITextEditorCommandListener _textEditor;
		private TextEditorSnapshot _snapshot;

		public AutoFormatCommand(ITextEditorCommandListener textEditor)
		{
			_textEditor = textEditor;
		}

		public void OnOptionChange(EditorOptions newOptions)
		{
			_currentOptions = newOptions;
		}

		public void OnTextEditorStateChange(TextEditorSnapshot snapshot)
		{
			_snapshot = snapshot;
		}

		public void OnExternalClick()
		{
			var autoFormatter = new AutoFormat();
			var formattedBuffer = autoFormatter.Format(_snapshot.Tokens, _currentOptions.IndentSize);
			_textEditor.OnAutoFormat(formattedBuffer);
		}
	}
}