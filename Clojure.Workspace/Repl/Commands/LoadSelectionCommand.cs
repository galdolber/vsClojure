using System;
using Clojure.Workspace.Menus;
using Clojure.Workspace.TextEditor;

namespace Clojure.Workspace.Repl.Commands
{
	public class LoadSelectionCommand : IEditorMenuCommandListener
	{
		private readonly IRepl _repl;

		public LoadSelectionCommand(IRepl repl)
		{
			_repl = repl;
		}

		public void Selected(TextEditorSnapshot snapshot)
		{
			_repl.Write(snapshot.Selection);
		}
	}
}