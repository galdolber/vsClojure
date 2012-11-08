using Clojure.Code.Editing.Indenting;
using Clojure.Workspace.TextEditor.Options;

namespace Clojure.Workspace.TextEditor.Commands
{
	public class SmartIndentCommand : ITextEditorStateChangeListener, IEditorOptionsChangedListener
	{
		private TextEditorSnapshot _snapshot;
		private EditorOptions _currentEditorOptions;

		public void Dispose()
		{
		}

		public int GetDesiredIndentation(int startPosition)
		{
			return new ClojureSmartIndent().GetDesiredIndentation(_snapshot.Tokens, startPosition, _currentEditorOptions.IndentSize);
		}

		public void OnTextEditorStateChange(TextEditorSnapshot snapshot)
		{
			_snapshot = snapshot;
		}

		public void OnOptionChange(EditorOptions newOptions)
		{
			_currentEditorOptions = newOptions;
		}
	}
}