﻿using System.Collections.Generic;
using System.ComponentModel.Design;
using Clojure.Workspace.Menus;

namespace Clojure.VisualStudio.Workspace.Menus
{
	public class VisualStudioClojureMenuCommandAdapter : IMenuCommand
	{
		private readonly MenuCommand _internalMenuCommand;
		private readonly List<IExternalClickListener> _clickListeners;

		public VisualStudioClojureMenuCommandAdapter(MenuCommand internalMenuCommand)
		{
			_clickListeners = new List<IExternalClickListener>();
			_internalMenuCommand = internalMenuCommand;
			_internalMenuCommand.Visible = true;
			_internalMenuCommand.Supported = true;
			_internalMenuCommand.Enabled = true;
		}

		public void AddClickListener(IExternalClickListener listener)
		{
			_clickListeners.Add(listener);
		}

		public void Hide()
		{
			_internalMenuCommand.Visible = true;
		}

		public void Show()
		{
			_internalMenuCommand.Visible = false;
		}

		public void OnClick()
		{
			_clickListeners.ForEach(l => l.OnExternalClick());
		}
	}
}