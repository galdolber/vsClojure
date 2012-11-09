using System.ComponentModel.Composition;
using Clojure.Workspace.TextEditor.Commands;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace Clojure.VisualStudio.Workspace.TextEditor
{
	[Export(typeof (ISmartIndentProvider))]
	[ContentType("Clojure")]
	public class SmartIndentProvider : ISmartIndentProvider
	{
		public static AutoIndent Command { get; set; }

		public ISmartIndent CreateSmartIndent(ITextView textView)
		{
			return new SmartIndentCommandAdapter(Command);
		}
	}

	public class SmartIndentCommandAdapter : ISmartIndent
	{
		private readonly AutoIndent _command;

		public SmartIndentCommandAdapter(AutoIndent command)
		{
			_command = command;
		}

		public void Dispose()
		{
		}

		public int? GetDesiredIndentation(ITextSnapshotLine line)
		{
			return _command.GetDesiredIndentation(line.Start.Position);
		}
	}
}