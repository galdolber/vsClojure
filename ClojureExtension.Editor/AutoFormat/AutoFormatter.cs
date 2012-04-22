using System.Collections.Generic;
using Clojure.Parsing;
using ClojureExtension.Editor.TextBuffer;
using ClojureExtension.Utilities;
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
			var autoFormatter = new Clojure.Code.Editing.AutoFormat();
			var formattedBuffer = autoFormatter.Format(_tokenizedBuffer.CurrentState, editorOptions.IndentSize);
			_textBuffer.SetText(formattedBuffer);
		}
	}
}