namespace Clojure.VisualStudio.Editor.Options
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
	}
}