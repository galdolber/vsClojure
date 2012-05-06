using System.Collections.Generic;
using Clojure.Code.Editing.Indenting;
using Clojure.Code.Parsing;
using Clojure.Code.State;
using Clojure.VisualStudio.Editor.Options;
using Clojure.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace Clojure.VisualStudio.Editor.AutoIndent
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