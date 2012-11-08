using Clojure.Code.Editing.Indenting;
using Clojure.Workspace.TextEditor.Options;

namespace Clojure.Workspace.TextEditor.Commands
{
	public class SmartIndentCommand : ISmartIndent, ITextEditorStateChangeListener, IEditorOptionsChangedListener
	{
		private TextEditorSnapshot _snapshot;
		private EditorOptions _currentEditorOptions;

		public void Dispose()
		{
		}

		public int? GetDesiredIndentation(ITextSnapshotLine line)
		{
			return new ClojureSmartIndent().GetDesiredIndentation(_snapshot.Tokens, line.Start.Position, _currentEditorOptions.IndentSize);
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