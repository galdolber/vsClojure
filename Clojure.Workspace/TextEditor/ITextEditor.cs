using System.Collections.Generic;
using Clojure.Workspace.Explorer;

namespace Clojure.Workspace.TextEditor
{
	public interface ITextEditor
	{
        void AddStateChangeListener(ITextEditorStateChangeListener listener);
	}
}