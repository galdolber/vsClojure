using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using Clojure.VisualStudio.Workspace.TextEditor;
using Microsoft.VisualStudio.Shell;

namespace Clojure.VisualStudio.Workspace.Menus
{
	public class ClojureEditorMenuCommandService : OleMenuCommandService, IActiveEditorChangeListener
	{
		private readonly List<MenuCommand> _commands;

		public ClojureEditorMenuCommandService(IServiceProvider serviceProvider) : base(serviceProvider)
		{
			_commands = new List<MenuCommand>();
		}

		public void OnActiveEditorChange(VisualStudioClojureTextView view)
		{
			_commands.FindAll(c => FindCommand(c.CommandID) == null).ToList().ForEach(AddCommand);
		}

		public void NonClojureEditorActivated()
		{
			_commands.ForEach(RemoveCommand);
		}

		public void Add(MenuCommand menuCommand)
		{
			_commands.Add(menuCommand);
			AddCommand(menuCommand);
		}
	}
}