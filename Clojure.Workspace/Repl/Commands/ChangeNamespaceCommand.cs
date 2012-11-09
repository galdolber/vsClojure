using System;
using Clojure.Workspace.Menus;
using Clojure.Workspace.Repl.Presentation;
using Clojure.Workspace.TextEditor;

namespace Clojure.Workspace.Repl.Commands
{
	public class ChangeNamespaceCommand : IEditorMenuCommandListener
	{
		private readonly IRepl _repl;

		public ChangeNamespaceCommand(IRepl repl)
		{
			_repl = repl;
		}

		public void Selected(TextEditorSnapshot snapshot)
		{
			_repl.ChangeNamespace(snapshot.Tokens);
		}
	}
}