using System;
using Clojure.Workspace.Menus;
using Clojure.Workspace.TextEditor;

namespace Clojure.Workspace.Repl.Commands
{
    public class LoadSelectionCommand : IExternalClickListener, ITextEditorStateChangeListener
	{
		private readonly IRepl _repl;
		private TextBufferSnapshot _snapshot;

		public LoadSelectionCommand(IRepl repl)
		{
			_repl = repl;
		}

		public void OnExternalClick()
		{
			_repl.Write(_snapshot.Selection);
		}

        public void OnTextEditorStatusChange(TextBufferSnapshot snapshot)
		{
            _snapshot = snapshot;
		}
	}
}