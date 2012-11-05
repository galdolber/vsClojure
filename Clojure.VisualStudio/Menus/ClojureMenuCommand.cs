using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Clojure.VisualStudio.Workspace;

namespace Clojure.VisualStudio.Repl
{
	public class ClojureMenuCommand : IEnvironmentListener
	{
		public static readonly CommandID LoadProjectIntoReplCommandId = new CommandID(Guids.GuidClojureExtensionCmdSet, 11);
		public static readonly CommandID WriteSelectionToReplCommandId = new CommandID(Guids.GuidClojureExtensionCmdSet, 15);
		private readonly MenuCommand _menuCommand;
		private readonly List<IMenuCommandListener> _listeners;

		public ClojureMenuCommand(CommandID commandId, IMenuCommandListener listener)
		{
			_menuCommand = new MenuCommand((sender, args) => OnClick(), commandId);
			_listeners = new List<IMenuCommandListener>() {listener};
		}

		private void OnClick()
		{
			_listeners.ForEach(l => l.OnMenuCommandClick());
		}

		public void RegisterWith(IMenuCommandService menuCommandService)
		{
			menuCommandService.AddCommand(_menuCommand);
		}

		public void EnvironmentStateChange(ClojureEnvironmentSnapshot snapshot)
		{
			_menuCommand.Visible = snapshot.State == ClojureEnvironmentState.ReplAndEditorActive;
		}
	}
}