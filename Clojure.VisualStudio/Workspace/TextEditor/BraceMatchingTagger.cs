using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Clojure.Code.Editing.BraceMatching;
using Clojure.Workspace.TextEditor;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace Clojure.VisualStudio.Workspace.TextEditor
{
	public class BraceMatchingTagger : ITagger<TextMarkerTag>, IClojureTextBufferStateListener, IClojureViewActionListener
	{
		private readonly ITextBuffer _textBuffer;
		private TextBufferSnapshot _snapshot;
		private int _caretPosition;
		public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

		public BraceMatchingTagger(ITextBuffer textBuffer)
		{
			_textBuffer = textBuffer;
			_snapshot = TextBufferSnapshot.Empty();
		}

		public IEnumerable<ITagSpan<TextMarkerTag>> GetTags(NormalizedSnapshotSpanCollection spans)
		{
			var bracePair = new MatchingBraceFinder().FindMatchingBraces(_snapshot.Tokens, _textBuffer.CurrentSnapshot.Length, _caretPosition);
			var tags = new LinkedList<ITagSpan<TextMarkerTag>>();
			if (bracePair.Start == null && bracePair.End == null) return tags;
			if (bracePair.Start == null) tags.AddLast(new TagSpan<TextMarkerTag>(new SnapshotSpan(_textBuffer.CurrentSnapshot, bracePair.End.StartIndex, bracePair.End.Token.Length), new TextMarkerTag("ClojureBraceNotFound")));
			if (bracePair.End == null) tags.AddLast(new TagSpan<TextMarkerTag>(new SnapshotSpan(_textBuffer.CurrentSnapshot, bracePair.Start.StartIndex, bracePair.Start.Token.Length), new TextMarkerTag("ClojureBraceNotFound")));

			if (bracePair.Start != null && bracePair.End != null)
			{
				tags.AddLast(new TagSpan<TextMarkerTag>(new SnapshotSpan(_textBuffer.CurrentSnapshot, bracePair.End.StartIndex, bracePair.End.Token.Length), new TextMarkerTag("ClojureBraceFound")));
				tags.AddLast(new TagSpan<TextMarkerTag>(new SnapshotSpan(_textBuffer.CurrentSnapshot, bracePair.Start.StartIndex, bracePair.Start.Token.Length), new TextMarkerTag("ClojureBraceFound")));
			}

			return tags;
		}

		private void InvalidateAllTags()
		{
			if (TagsChanged != null) TagsChanged(this, new SnapshotSpanEventArgs(new SnapshotSpan(_textBuffer.CurrentSnapshot, 0, _textBuffer.CurrentSnapshot.Length)));
		}

		public void TokensChanged(TextBufferSnapshot snapshot, BufferDiffGram diffGram)
		{
			_snapshot = snapshot;
			InvalidateAllTags();
		}

		public void BufferChanged(string newText)
		{
		}

		public void OnCaretPositionChange(int newPosition)
		{
			_caretPosition = newPosition;
			InvalidateAllTags();
		}
	}

	[Export(typeof (IViewTaggerProvider))]
	[ContentType("Clojure")]
	[TagType(typeof (TextMarkerTag))]
	public class BraceMatchingTaggerProvider : IViewTaggerProvider
	{
		public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
		{
			return buffer.Properties.GetProperty(typeof (BraceMatchingTagger)) as ITagger<T>;
		}
	}

	[Export(typeof (EditorFormatDefinition))]
	[Name("ClojureBraceNotFound")]
	[UserVisible(true)]
	internal class BraceNotFoundFormatDefinition : MarkerFormatDefinition
	{
		public BraceNotFoundFormatDefinition()
		{
			BackgroundColor = Colors.MistyRose;
			DisplayName = "Clojure - Brace Not Found";
			ZOrder = 5;
		}
	}

	[Export(typeof (EditorFormatDefinition))]
	[Name("ClojureBraceFound")]
	[UserVisible(true)]
	internal class BraceFoundFormatDefinition : MarkerFormatDefinition
	{
		public BraceFoundFormatDefinition()
		{
			BackgroundColor = Colors.LightBlue;
			DisplayName = "Clojure - Brace Found";
			ZOrder = 5;
		}
	}
}