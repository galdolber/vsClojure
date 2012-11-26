using System.Collections.Generic;

namespace Clojure.Workspace.TextEditor
{
	public interface IClojureTextBufferStateListener
	{
		void TokensChanged(BufferDiffGram diffGram);
		void BufferChanged(string newText);
		void ReplaceSelectedLines(List<string> newLines);
	}
}