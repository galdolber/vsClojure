using System;
using System.Collections.Generic;
using Clojure.Code.Parsing;
using Clojure.Workspace.Menus;
using Clojure.Workspace.Repl.Presentation;
using Clojure.Workspace.TextEditor;

namespace Clojure.Workspace.Repl.Commands
{
    public class ChangeNamespaceCommand : IExternalClickListener, ITextEditorStateChangeListener
	{
		private readonly IRepl _repl;
		private TextBufferSnapshot _snapshot;

		public ChangeNamespaceCommand(IRepl repl)
		{
			_repl = repl;
		}

		public void OnExternalClick()
		{
            if (_snapshot == null)
            {
                return;
		}

			_repl.ChangeNamespace(_snapshot.Tokens);
		}

        public void OnTextEditorStatusChange(TextBufferSnapshot snapshot)
        {
            _snapshot = snapshot;
        }
	}
}