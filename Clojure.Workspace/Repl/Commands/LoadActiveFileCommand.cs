using Clojure.System.Collections;
using Clojure.Workspace.Menus;
using Clojure.Workspace.TextEditor;

namespace Clojure.Workspace.Repl.Commands
{
	public class LoadActiveFileCommand : ITextEditorStateChangeListener, IExternalClickListener
	{
		private readonly IRepl _repl;
		private TextEditorSnapshot _snapshot;

		public LoadActiveFileCommand(IRepl repl)
		{
			_repl = repl;
		}

		public void OnTextEditorStateChange(TextEditorSnapshot snapshot)
		{
			_snapshot = snapshot;
		}

		public void OnExternalClick()
		{
			_repl.LoadFiles(_snapshot.FilePath.SingletonAsList());
		}
	}
}