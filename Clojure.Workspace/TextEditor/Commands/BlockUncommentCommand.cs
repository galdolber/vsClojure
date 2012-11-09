using Clojure.Code.Editing.Commenting;

namespace Clojure.Workspace.TextEditor.Commands
{
	public class BlockUncommentCommand : IEditorMenuCommandListener
	{
		private readonly ITextEditorCommandListener _textEditorCommandListener;

		public BlockUncommentCommand(ITextEditorCommandListener textEditorCommandListener)
		{
			_textEditorCommandListener = textEditorCommandListener;
		}

		public void Selected(TextEditorSnapshot snapshot)
		{
			_textEditorCommandListener.ReplaceSelectedLines(new BlockUncomment().Execute(snapshot.SelectedLines));
		}
	}
}
