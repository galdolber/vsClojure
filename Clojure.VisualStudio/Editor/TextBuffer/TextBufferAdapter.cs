using System;
using System.Collections.Generic;
using System.Linq;
using Clojure.Code.Editing.TextBuffer;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace Clojure.VisualStudio.Editor.TextBuffer
{
	public class TextBufferAdapter : ITextBufferAdapter
	{
		private readonly ITextView _textView;

		public TextBufferAdapter(ITextView textView)
		{
			_textView = textView;
		}

		public string GetText(int startPosition)
		{
			return _textView.TextBuffer.CurrentSnapshot.GetText().Substring(startPosition);
		}

		public int Length
		{
			get { return _textView.TextBuffer.CurrentSnapshot.Length; }
		}
	}
}