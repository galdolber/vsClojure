﻿using System;
using Clojure.Workspace.Menus;
using Clojure.Workspace.Repl.Presentation;
using Clojure.Workspace.TextEditor;

namespace Clojure.Workspace.Repl.Commands
{
	public class ChangeNamespaceCommand : ITextEditorStateChangeListener, IExternalClickListener
	{
		private readonly IRepl _repl;
		private TextEditorSnapshot _snapshot;

		public ChangeNamespaceCommand(IRepl repl)
		{
			_repl = repl;
		}

		public void OnTextEditorStateChange(TextEditorSnapshot snapshot)
		{
			_snapshot = snapshot;
		}

		public void OnExternalClick()
		{
			_repl.ChangeNamespace(_snapshot.Tokens);
		}
	}
}