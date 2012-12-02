using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Clojure.Code.Editing.Indenting;
using Clojure.Workspace.TextEditor;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace Clojure.VisualStudio.Workspace.TextEditor.SmartIndent
{
	public class ClojureAutoIndent : ISmartIndent, IClojureTextBufferStateListener
	{
		private TextEditorSnapshot _snapshot;

		public ClojureAutoIndent()
		{
			_snapshot = TextEditorSnapshot.Empty;
		}

		public void Dispose()
		{
			
		}

		public int? GetDesiredIndentation(ITextSnapshotLine line)
		{
			return new ClojureSmartIndent().GetDesiredIndentation(_snapshot.Tokens, line.Start.Position, 2);
		}

		public void TokensChanged(TextEditorSnapshot snapshot, BufferDiffGram diffGram)
		{
			_snapshot = snapshot;
		}

		public void BufferChanged(string newText)
		{
		}
	}

	[Export(typeof(ISmartIndentProvider))]
	[ContentType("Clojure")]
	public class SmartIndentProvider : ISmartIndentProvider
	{
		public ISmartIndent CreateSmartIndent(ITextView textView)
		{
			return textView.TextBuffer.Properties.GetProperty<ClojureAutoIndent>(typeof(ClojureAutoIndent));
		}
	}
}
