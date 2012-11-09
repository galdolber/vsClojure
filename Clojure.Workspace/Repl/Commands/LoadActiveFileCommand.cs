using System;
using Clojure.System.Collections;
using Clojure.Workspace.Menus;
using Clojure.Workspace.Repl.Presentation;
using Clojure.Workspace.TextEditor;

namespace Clojure.Workspace.Repl.Commands
{
	public class LoadActiveFileCommand : IEditorMenuCommandListener
	{
		private readonly IRepl _repl;

		public LoadActiveFileCommand(IRepl repl)
		{
			_repl = repl;
		}

		public void Selected(TextEditorSnapshot snapshot)
		{
			_repl.LoadFiles(snapshot.FilePath.SingletonAsList());
		}
	}
}