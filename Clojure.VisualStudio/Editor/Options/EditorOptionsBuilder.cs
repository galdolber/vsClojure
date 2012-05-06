using Clojure.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Editor;

namespace Clojure.VisualStudio.Editor.Options
{
	public class EditorOptionsBuilder : IProvider<EditorOptions>
	{
		private readonly IEditorOptions _editorOptions;

		public EditorOptionsBuilder(IEditorOptions editorOptions)
		{
			_editorOptions = editorOptions;
		}

		public EditorOptions Get()
		{
			return new EditorOptions(_editorOptions.GetOptionValue<int>(new IndentSize().Key));
		}
	}
}
