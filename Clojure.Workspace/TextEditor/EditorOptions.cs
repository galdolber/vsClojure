namespace Clojure.Workspace.TextEditor.Options
{
	public class EditorOptions
	{
		private readonly int _indentSize;

		public EditorOptions(int indentSize)
		{
			_indentSize = indentSize;
		}

		public int IndentSize
		{
			get { return _indentSize; }
		}

		public static EditorOptions Empty = new EditorOptions(0);
	}
}