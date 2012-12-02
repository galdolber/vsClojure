using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clojure.VisualStudio.Workspace.TextEditor.View
{
	public interface IClojureViewActionListener
	{
		void OnCaretPositionChange(int newPosition);
	}
}
