using System;
using Clojure.Code.Editing.Commenting;
using Clojure.Workspace.Menus;

namespace Clojure.Workspace.TextEditor.Commands
{
	public class BlockCommentCommand : ITextEditorStateChangeListener, IExternalClickListener
	{
		private readonly ITextEditorCommandListener _listener;
		private TextEditorSnapshot _snapshot;

		public BlockCommentCommand(ITextEditorCommandListener listener)
		{
			_listener = listener;
		}

		public void OnTextEditorStateChange(TextEditorSnapshot snapshot)
		{
			_snapshot = snapshot;
		}

		public void OnExternalClick()
		{
			_listener.ReplaceSelectedLines(new BlockComment().Execute(_snapshot.SelectedLines));
		}
	}
}
