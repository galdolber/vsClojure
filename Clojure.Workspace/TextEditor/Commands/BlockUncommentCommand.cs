using System;
using Clojure.Code.Editing.Commenting;
using Clojure.Workspace.Menus;

namespace Clojure.Workspace.TextEditor.Commands
{
	public class BlockUncommentCommand : ITextEditorStateChangeListener, IExternalClickListener
	{
		private readonly ITextEditorCommandListener _textEditorCommandListener;
		private TextEditorSnapshot _snapshot;

		public BlockUncommentCommand(ITextEditorCommandListener textEditorCommandListener)
		{
			_textEditorCommandListener = textEditorCommandListener;
		}

		public void OnTextEditorStateChange(TextEditorSnapshot snapshot)
		{
			_snapshot = snapshot;
		}

		public void OnExternalClick()
		{
			_textEditorCommandListener.ReplaceSelectedLines(new BlockUncomment().Execute(_snapshot.SelectedLines));
		}
	}
}
