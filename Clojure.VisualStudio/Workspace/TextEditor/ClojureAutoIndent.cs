using System.ComponentModel.Composition;
using Clojure.Code.Editing.Indenting;
using Clojure.Workspace.TextEditor;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace Clojure.VisualStudio.Workspace.TextEditor
{
	public class ClojureAutoIndent : ISmartIndent
	{
		private readonly ClojureTextBuffer _clojureTextBuffer;

		public ClojureAutoIndent(ClojureTextBuffer clojureTextBuffer)
		{
			_clojureTextBuffer = clojureTextBuffer;
		}

		public void Dispose()
		{
		}

		public int? GetDesiredIndentation(ITextSnapshotLine line)
		{
			return new ClojureSmartIndent().GetDesiredIndentation(_clojureTextBuffer.GetSnapshot().Tokens, line.Start.Position, 2);
		}
	}

	[Export(typeof (ISmartIndentProvider))]
	[ContentType("Clojure")]
	public class SmartIndentProvider : ISmartIndentProvider
	{
		public ISmartIndent CreateSmartIndent(ITextView textView)
		{
			return textView.TextBuffer.Properties.GetProperty<ClojureAutoIndent>(typeof (ClojureAutoIndent));
		}
	}
}