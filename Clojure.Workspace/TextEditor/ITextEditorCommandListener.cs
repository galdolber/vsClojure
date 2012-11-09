using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clojure.Workspace.TextEditor
{
	public interface ITextEditorCommandListener
	{
		void ReplaceSelectedLines(List<string> newLines);
		void OnAutoFormat(string text);
	}
}
