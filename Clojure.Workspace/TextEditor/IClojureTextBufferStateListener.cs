using System.Collections.Generic;

namespace Clojure.Workspace.TextEditor
{
	public interface IClojureTextBufferStateListener
	{
		void TokensChanged(TextBufferSnapshot snapshot, BufferDiffGram diffGram);
		void BufferChanged(string newText);
	}
}