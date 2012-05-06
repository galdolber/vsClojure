using System.Collections.Generic;
using System.ComponentModel.Composition;
using Clojure.Code.Editing.PartialUpdate;
using Clojure.Code.Parsing;
using Clojure.Code.State;
using Clojure.VisualStudio.Editor.InputHandling;
using Clojure.VisualStudio.Editor.TextBuffer;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace Clojure.VisualStudio.Editor.Tagger
{
	[Export(typeof (IViewTaggerProvider))]
	[ContentType("Clojure")]
	[TagType(typeof (ClojureTokenTag))]
	public class ClojureTagProvider : IViewTaggerProvider
	{
		public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
		{
			Entity<LinkedList<Token>> tokenizedBuffer = TokenizedBufferBuilder.TokenizedBuffers[buffer];
			ClojureTokenTagger tagger = new ClojureTokenTagger(buffer, tokenizedBuffer);
			BufferTextChangeHandler textChangeHandler = new BufferTextChangeHandler(new TextBufferAdapter(textView), tokenizedBuffer);
			TextChangeAdapter textChangeAdapter = new TextChangeAdapter(textChangeHandler);
			buffer.Changed += textChangeAdapter.OnTextChange;
			textChangeHandler.TokenChanged += tagger.OnTokenChange;
			return tagger as ITagger<T>;
		}
	}
}