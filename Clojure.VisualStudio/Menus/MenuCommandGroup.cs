using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Clojure.VisualStudio.Environment;
using Clojure.VisualStudio.Repl;

namespace Clojure.VisualStudio.Menus
{
	public class MenuCommandGroup : IEnvironmentListener
	{
		private readonly IMenuCommandService _menuCommandService;
		private readonly List<MenuCommand> _menuCommands;

		public MenuCommandGroup(IMenuCommandService menuCommandService, List<MenuCommand> menuCommands)
		{
			_menuCommandService = menuCommandService;
			_menuCommands = menuCommands;
		}

		private void Enable()
		{
			foreach (var menuCommand in _menuCommands)
			{
				_menuCommandService.AddCommand(menuCommand);
			}
		}

		private void Disable()
		{
			foreach (var menuCommand in _menuCommands)
			{
				_menuCommandService.RemoveCommand(menuCommand);
			}
		}

		public void EnvironmentStateChange(ClojureEnvironmentSnapshot snapshot)
		{
			if (snapshot.State == ClojureEnvironmentState.ReplAndEditorActive) Enable();
			else Disable();
		}
	}
}