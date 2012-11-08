using System;
using System.Collections.Generic;
using Clojure.VisualStudio.Workspace.TextEditor.Options;

namespace Clojure.VisualStudio.Workspace.TextEditor.Commands
{
	public class AutoFormatCommand : ITextEditorStateChangeListener, IEditorOptionsChangedListener
	{
		private TextEditorSnapshot _snapshot;
		private readonly List<IAutoFormatListener> _commandListeners;
		private EditorOptions _currentOptions;

		public AutoFormatCommand()
		{
			_commandListeners = new List<IAutoFormatListener>();
		}

		public void AddAutoFormatListener(IAutoFormatListener listener)
		{
			_commandListeners.Add(listener);
		}

		public void Format()
		{
			var autoFormatter = new Clojure.Code.Editing.Formatting.AutoFormat();
			var formattedBuffer = autoFormatter.Format(_snapshot.Tokens, _currentOptions.IndentSize);
			_commandListeners.ForEach(l => l.OnAutoFormat(formattedBuffer));
		}

		public void OnTextEditorStateChange(TextEditorSnapshot snapshot)
		{
			_snapshot = snapshot;
		}

		public void OnOptionChange(EditorOptions newOptions)
		{
			_currentOptions = newOptions;
		}
	}
}