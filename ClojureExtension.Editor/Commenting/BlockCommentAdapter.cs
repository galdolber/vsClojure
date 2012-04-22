using Clojure.Code.Editing.Commenting;
using ClojureExtension.Editor.TextBuffer;

namespace ClojureExtension.Editor.Commenting
{
	public class BlockCommentAdapter
	{
		private readonly ITextBufferAdapter _textBuffer;

		public BlockCommentAdapter(ITextBufferAdapter textBuffer)
		{
			_textBuffer = textBuffer;
		}

		public void Execute()
		{
			_textBuffer.ReplaceSelectedLines(new BlockComment().Execute(_textBuffer.GetSelectedLines()));
		}
	}
}