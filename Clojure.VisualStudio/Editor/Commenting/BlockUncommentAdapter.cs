using Clojure.Code.Editing.Commenting;
using Clojure.Code.Editing.TextBuffer;

namespace Clojure.VisualStudio.Editor.Commenting
{
	public class BlockUncommentAdapter
	{
		private readonly ITextBufferAdapter _textBuffer;

		public BlockUncommentAdapter(ITextBufferAdapter textBuffer)
		{
			_textBuffer = textBuffer;
		}

		public void Execute()
		{
			_textBuffer.ReplaceSelectedLines(new BlockUncomment().Execute(_textBuffer.GetSelectedLines()));
		}
	}
}