using System.Collections.Generic;
using Clojure.Code.Editing.Indenting;
using Clojure.Code.Parsing;
using Clojure.Code.State;
using ClojureExtension.Utilities;
using Microsoft.ClojureExtension.Editor.Options;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace Microsoft.ClojureExtension.Editor.AutoIndent
{
	public class SmartIndentAdapter : ISmartIndent
	{
		private readonly Entity<LinkedList<Token>> _buffer;
		private readonly IProvider<EditorOptions> _optionsBuilder;

		public SmartIndentAdapter(Entity<LinkedList<Token>> buffer, IProvider<EditorOptions> optionsBuilder)
		{
			_buffer = buffer;
			_optionsBuilder = optionsBuilder;
		}

		public void Dispose()
		{
		}

		public int? GetDesiredIndentation(ITextSnapshotLine line)
		{
			return new ClojureSmartIndent().GetDesiredIndentation(_buffer.CurrentState, line.Start.Position, _optionsBuilder.Get().IndentSize);
		}
	}
}