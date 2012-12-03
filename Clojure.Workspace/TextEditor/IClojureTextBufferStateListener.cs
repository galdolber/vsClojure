namespace Clojure.Workspace.TextEditor
{
	public interface IClojureTextBufferStateListener
	{
		void TokensChanged(TextBufferSnapshot snapshot, BufferDiffGram diffGram);
		void CaretChanged(TextBufferSnapshot snapshot);
	}
}