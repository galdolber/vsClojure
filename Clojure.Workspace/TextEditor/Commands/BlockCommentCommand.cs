using Clojure.Code.Editing.Commenting;

namespace Clojure.Workspace.TextEditor.Commands
{
	public class BlockCommentCommand : IEditorMenuCommandListener
	{
		private readonly ITextEditorCommandListener _listener;

		public BlockCommentCommand(ITextEditorCommandListener listener)
		{
			_listener = listener;
		}

		public void Selected(TextEditorSnapshot snapshot)
		{
			_listener.ReplaceSelectedLines(new BlockComment().Execute(snapshot.SelectedLines));
		}
	}
}
