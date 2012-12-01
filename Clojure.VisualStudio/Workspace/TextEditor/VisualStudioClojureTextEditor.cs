using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Clojure.Code.Editing.Indenting;
using Clojure.Code.Editing.PartialUpdate;
using Clojure.Code.Parsing;
using Clojure.VisualStudio.Editor.Tagger;
using Clojure.Workspace.TextEditor;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace Clojure.VisualStudio.Workspace.TextEditor
{
	public class VisualStudioClojureTextEditor : IUserActionSource, IClojureTextBufferStateListener, ITagger<ClojureTokenTag>, ISmartIndent
	{
		private readonly ITextBuffer _currentBuffer;
		private readonly ITextView _currentWpfTextView;
		private readonly List<IUserActionListener> _actionListeners;

		private TextEditorSnapshot _snapshot;
		public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

		public static Dictionary<ITextView, VisualStudioClojureTextEditor> Editors = new Dictionary<ITextView, VisualStudioClojureTextEditor>();

		public VisualStudioClojureTextEditor(ITextView view)
		{
			_currentWpfTextView = view;
			_currentBuffer = _currentWpfTextView.TextBuffer;
			_currentBuffer.Changed += DocumentEdited;
			_actionListeners = new List<IUserActionListener>();
		}

		private void DocumentEdited(object sender, TextContentChangedEventArgs args)
		{
			var changeData = args.Changes.Select(
				change => new TextChangeData(change.OldPosition, change.Delta, Math.Max(change.NewSpan.Length, change.OldSpan.Length))).ToList();

			_actionListeners.ForEach(l => l.Edit(changeData, _currentBuffer.CurrentSnapshot.GetText()));
		}

		public void AddUserActionListener(IUserActionListener listener)
		{
			_actionListeners.Add(listener);
		}

		public void CommentSelectedLines()
		{
			int startPosition = _currentWpfTextView.Selection.Start.Position.GetContainingLine().Start.Position;
			int endPosition = _currentWpfTextView.Selection.End.Position.GetContainingLine().End.Position;
			_actionListeners.ForEach(l => l.CommentLines(startPosition, endPosition));
		}

		public void UncommentSelectedLines()
		{
			int startPosition = _currentWpfTextView.Selection.Start.Position.GetContainingLine().Start.Position;
			int endPosition = _currentWpfTextView.Selection.End.Position.GetContainingLine().End.Position;
			_actionListeners.ForEach(l => l.UncommentLines(startPosition, endPosition));
		}

		public void BufferChanged(string newText)
		{
			_currentBuffer.Replace(new Span(0, _currentBuffer.CurrentSnapshot.Length), newText);
		}

		public void TokensChanged(TextEditorSnapshot snapshot, BufferDiffGram diffGram)
		{
			_snapshot = snapshot;

			var startIndex = diffGram.NewTokens.First().StartIndex;
			var endIndex = diffGram.NewTokens.Last().StartIndex + diffGram.NewTokens.Last().Token.Length;
			var span = new SnapshotSpan(_currentBuffer.CurrentSnapshot, startIndex, endIndex);
			TagsChanged(this, new SnapshotSpanEventArgs(span));
		}

		public IEnumerable<ITagSpan<ClojureTokenTag>> GetTags(NormalizedSnapshotSpanCollection spans)
		{
			var tagSpans = new LinkedList<TagSpan<ClojureTokenTag>>();

			foreach (var curSpan in spans)
			{
				foreach (var intersectingTokenData in _snapshot.Tokens.Intersection(curSpan.Start.Position, curSpan.Length))
				{
					var storedTokenSpan = new SnapshotSpan(curSpan.Snapshot, new Span(intersectingTokenData.StartIndex, intersectingTokenData.Token.Length));
					tagSpans.AddLast(new TagSpan<ClojureTokenTag>(storedTokenSpan, new ClojureTokenTag(intersectingTokenData.Token)));
				}
			}

			return tagSpans;
		}

		public void Dispose()
		{
		}

		public int? GetDesiredIndentation(ITextSnapshotLine line)
		{
			return new ClojureSmartIndent().GetDesiredIndentation(_snapshot.Tokens, line.Start.Position, 2);
		}
	}

	[Export(typeof (IViewTaggerProvider))]
	[ContentType("Clojure")]
	[TagType(typeof (ClojureTokenTag))]
	public class ClojureTagProvider : IViewTaggerProvider
	{
		public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
		{
			return VisualStudioClojureTextEditor.Editors[textView] as ITagger<T>;
		}
	}

	[Export(typeof (ISmartIndentProvider))]
	[ContentType("Clojure")]
	public class SmartIndentProvider : ISmartIndentProvider
	{
		public ISmartIndent CreateSmartIndent(ITextView textView)
		{
			return VisualStudioClojureTextEditor.Editors[textView];
		}
	}
}