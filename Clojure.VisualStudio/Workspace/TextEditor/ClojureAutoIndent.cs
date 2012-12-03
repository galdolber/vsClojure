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
		private readonly VisualStudioClojureTextBuffer _clojureTextBuffer;

		public ClojureAutoIndent(VisualStudioClojureTextBuffer clojureTextBuffer)
		{
			_clojureTextBuffer = clojureTextBuffer;
		}

		public void Dispose()
		{
		}

		public int? GetDesiredIndentation(ITextSnapshotLine line)
		{
			return new ClojureSmartIndent().GetDesiredIndentation(_clojureTextBuffer.GetTokenSnapshot().Tokens, line.Start.Position, 2);
		}
	}

	[Export(typeof (ISmartIndentProvider))]
	[ContentType("Clojure")]
	public class SmartIndentProvider : ISmartIndentProvider
	{
		public ISmartIndent CreateSmartIndent(ITextView textView)
		{
			var clojureTextBuffer = textView.TextBuffer.Properties.GetProperty<VisualStudioClojureTextBuffer>(typeof(VisualStudioClojureTextBuffer));
			return new ClojureAutoIndent(clojureTextBuffer);
		}
	}
}