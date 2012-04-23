using System;
using System.Collections.Generic;
using Clojure.Code.Editing.BraceMatching;
using Clojure.Code.Parsing;
using Clojure.Code.State;
using ClojureExtension.Editor.TextBuffer;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;

namespace Microsoft.ClojureExtension.Editor.BraceMatching
{
	public class BraceMatchingTagger : ITagger<TextMarkerTag>
	{
		private readonly ITextView _textView;
		private readonly Entity<LinkedList<Token>> _tokenizedBuffer;
		public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

		public BraceMatchingTagger(ITextView textView, Entity<LinkedList<Token>> tokenizedBuffer)
		{
			_textView = textView;
			_tokenizedBuffer = tokenizedBuffer;
		}

		public void InvalidateAllTags()
		{
			TagsChanged(this, new SnapshotSpanEventArgs(new SnapshotSpan(_textView.TextBuffer.CurrentSnapshot, 0, _textView.TextBuffer.CurrentSnapshot.Length)));
		}

		public IEnumerable<ITagSpan<TextMarkerTag>> GetTags(NormalizedSnapshotSpanCollection spans)
		{
			var caretPosition = _textView.Caret.Position.BufferPosition.Position;

			var bracePair = new MatchingBraceFinder().FindMatchingBraces(
				_tokenizedBuffer.CurrentState,
				_textView.TextBuffer.CurrentSnapshot.Length,
				caretPosition);

			var tags = new LinkedList<ITagSpan<TextMarkerTag>>();

			if (bracePair.Start == null && bracePair.End == null) return tags;
			if (bracePair.Start == null) tags.AddLast(new TagSpan<TextMarkerTag>(new SnapshotSpan(_textView.TextBuffer.CurrentSnapshot, bracePair.End.StartIndex, bracePair.End.Token.Length), new TextMarkerTag("ClojureBraceNotFound")));
			if (bracePair.End == null) tags.AddLast(new TagSpan<TextMarkerTag>(new SnapshotSpan(_textView.TextBuffer.CurrentSnapshot, bracePair.Start.StartIndex, bracePair.Start.Token.Length), new TextMarkerTag("ClojureBraceNotFound")));

			if (bracePair.Start != null && bracePair.End != null)
			{
				tags.AddLast(new TagSpan<TextMarkerTag>(new SnapshotSpan(_textView.TextBuffer.CurrentSnapshot, bracePair.End.StartIndex, bracePair.End.Token.Length), new TextMarkerTag("ClojureBraceFound")));
				tags.AddLast(new TagSpan<TextMarkerTag>(new SnapshotSpan(_textView.TextBuffer.CurrentSnapshot, bracePair.Start.StartIndex, bracePair.Start.Token.Length), new TextMarkerTag("ClojureBraceFound")));
			}

			return tags;
		}
	}
}