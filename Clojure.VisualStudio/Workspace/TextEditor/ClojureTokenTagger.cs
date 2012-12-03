using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Clojure.Code.Parsing;
using Clojure.Workspace.TextEditor;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace Clojure.VisualStudio.Workspace.TextEditor
{
	public class ClojureTokenTagger : ITagger<ClojureTokenTag>, IClojureTextBufferStateListener
	{
		private readonly VisualStudioClojureTextBuffer _clojureTextBuffer;
		public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

		public ClojureTokenTagger(VisualStudioClojureTextBuffer clojureTextBuffer)
		{
			_clojureTextBuffer = clojureTextBuffer;
		}

		public IEnumerable<ITagSpan<ClojureTokenTag>> GetTags(NormalizedSnapshotSpanCollection spans)
		{
			var tagSpans = new LinkedList<TagSpan<ClojureTokenTag>>();
			var snapshot = _clojureTextBuffer.GetTokenSnapshot();

			foreach (var curSpan in spans)
			{
				foreach (var intersectingTokenData in snapshot.Tokens.Intersection(curSpan.Start.Position, curSpan.Length))
				{
					var storedTokenSpan = new SnapshotSpan(curSpan.Snapshot, new Span(intersectingTokenData.StartIndex, intersectingTokenData.Token.Length));
					tagSpans.AddLast(new TagSpan<ClojureTokenTag>(storedTokenSpan, new ClojureTokenTag(intersectingTokenData.Token)));
				}
			}

			return tagSpans;
		}

		public void TokensChanged(TextBufferSnapshot snapshot, BufferDiffGram diffGram)
		{
			if (TagsChanged == null) return;
			var startIndex = diffGram.NewTokens.First().StartIndex;
			var endIndex = diffGram.NewTokens.Last().StartIndex + diffGram.NewTokens.Last().Token.Length;
			var span = new SnapshotSpan(_clojureTextBuffer.GetTextSnapshot(), startIndex, endIndex - startIndex);
			TagsChanged(this, new SnapshotSpanEventArgs(span));
		}

		public void CaretChanged(TextBufferSnapshot snapshot) { }
	}

	[Export(typeof (IViewTaggerProvider))]
	[ContentType("Clojure")]
	[TagType(typeof (ClojureTokenTag))]
	public class ClojureTagProvider : IViewTaggerProvider
	{
		public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
		{
			var clojureTextBuffer = buffer.Properties.GetProperty<VisualStudioClojureTextBuffer>(typeof(VisualStudioClojureTextBuffer));
			var tokenTagger = new ClojureTokenTagger(clojureTextBuffer);
			clojureTextBuffer.AddStateChangeListener(tokenTagger);
			return tokenTagger as ITagger<T>;
		}
	}

	public class ClojureTokenTag : ITag
	{
		private readonly Token _token;

		public Token Token
		{
			get { return _token; }
		}

		public ClojureTokenTag(Token token)
		{
			_token = token;
		}
	}
}