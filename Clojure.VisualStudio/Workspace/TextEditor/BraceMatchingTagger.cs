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
	public class BraceMatchingTagger : ITagger<TextMarkerTag>, IClojureTextBufferStateListener
	{
		private readonly VisualStudioClojureTextBuffer _clojureTextBuffer;
		public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

		public BraceMatchingTagger(VisualStudioClojureTextBuffer clojureTextBuffer)
		{
			_clojureTextBuffer = clojureTextBuffer;
		}

		public IEnumerable<ITagSpan<TextMarkerTag>> GetTags(NormalizedSnapshotSpanCollection spans)
		{
			var textSnapshot = _clojureTextBuffer.GetTextSnapshot();
			var tokenSnapshot = _clojureTextBuffer.GetTokenSnapshot();

			var bracePair = new MatchingBraceFinder().FindMatchingBraces(tokenSnapshot.Tokens, textSnapshot.Length, tokenSnapshot.CaretPosition);
			var tags = new LinkedList<ITagSpan<TextMarkerTag>>();
			if (bracePair.Start == null && bracePair.End == null) return tags;
			if (bracePair.Start == null) tags.AddLast(new TagSpan<TextMarkerTag>(new SnapshotSpan(textSnapshot, bracePair.End.StartIndex, bracePair.End.Token.Length), new TextMarkerTag("ClojureBraceNotFound")));
			if (bracePair.End == null) tags.AddLast(new TagSpan<TextMarkerTag>(new SnapshotSpan(textSnapshot, bracePair.Start.StartIndex, bracePair.Start.Token.Length), new TextMarkerTag("ClojureBraceNotFound")));

			if (bracePair.Start != null && bracePair.End != null)
			{
				tags.AddLast(new TagSpan<TextMarkerTag>(new SnapshotSpan(textSnapshot, bracePair.End.StartIndex, bracePair.End.Token.Length), new TextMarkerTag("ClojureBraceFound")));
				tags.AddLast(new TagSpan<TextMarkerTag>(new SnapshotSpan(textSnapshot, bracePair.Start.StartIndex, bracePair.Start.Token.Length), new TextMarkerTag("ClojureBraceFound")));
			}

			return tags;
		}

		private void InvalidateAllTags()
		{
			if (TagsChanged == null) return;
			var textSnapshot = _clojureTextBuffer.GetTextSnapshot();
			TagsChanged(this, new SnapshotSpanEventArgs(new SnapshotSpan(textSnapshot, 0, textSnapshot.Length)));
		}

		public void TokensChanged(TextBufferSnapshot snapshot, BufferDiffGram diffGram)
		{
			InvalidateAllTags();
		}

		public void CaretChanged(TextBufferSnapshot snapshot)
		{
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
			var clojureTextBuffer = buffer.Properties.GetProperty<VisualStudioClojureTextBuffer>(typeof (VisualStudioClojureTextBuffer));
			var braceMatchingTagger = new BraceMatchingTagger(clojureTextBuffer);
			clojureTextBuffer.AddStateChangeListener(braceMatchingTagger);
			return braceMatchingTagger as ITagger<T>;
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