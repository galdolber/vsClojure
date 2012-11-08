using System.ComponentModel.Composition;
using Clojure.VisualStudio.Workspace.TextEditor.Options;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace Clojure.VisualStudio.Editor.AutoIndent
{
	[Export(typeof (ISmartIndentProvider))]
	[ContentType("Clojure")]
	public class SmartIndentProvider : ISmartIndentProvider
	{
		public static SmartIndentCommand Command { get; set; }

		public ISmartIndent CreateSmartIndent(ITextView textView)
		{
			return Command;
		}
	}
}