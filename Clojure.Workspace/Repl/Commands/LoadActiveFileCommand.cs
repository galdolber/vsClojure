using Clojure.Base.Collections;
using Clojure.Workspace.Menus;
using Clojure.Workspace.TextEditor;

namespace Clojure.Workspace.Repl.Commands
{
    public class LoadActiveFileCommand : IExternalClickListener, ITextEditorStateChangeListener
	{
		private readonly IRepl _repl;
		private TextBufferSnapshot _snapshot;

		public LoadActiveFileCommand(IRepl repl)
		{
			_repl = repl;
		}

		public void OnExternalClick()
		{
            if (_snapshot == null)
            {
                return;
		}

			_repl.LoadFiles(_snapshot.FilePath.SingletonAsList());
		}

        public void OnTextEditorStatusChange(TextBufferSnapshot snapshot)
        {        
            _snapshot = snapshot;
        }
	}
}