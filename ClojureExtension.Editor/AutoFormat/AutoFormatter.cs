using System.Collections.Generic;
using Clojure.Code.Editing.TextBuffer;
using Clojure.Code.Parsing;
using Clojure.Code.State;
using Microsoft.ClojureExtension.Editor.Options;

namespace Microsoft.ClojureExtension.Editor.AutoFormat
{
	public class AutoFormatter
	{
		private readonly ITextBufferAdapter _textBuffer;
		private readonly Entity<LinkedList<Token>> _tokenizedBuffer;

		public AutoFormatter(ITextBufferAdapter textBuffer, Entity<LinkedList<Token>> tokenizedBuffer)
		{
			_textBuffer = textBuffer;
			_tokenizedBuffer = tokenizedBuffer;
		}

		public void Format(EditorOptions editorOptions)
		{
			var autoFormatter = new Clojure.Code.Editing.Formatting.AutoFormat();
			var formattedBuffer = autoFormatter.Format(_tokenizedBuffer.CurrentState, editorOptions.IndentSize);
			_textBuffer.SetText(formattedBuffer);
		}
	}
}