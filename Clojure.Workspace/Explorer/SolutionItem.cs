namespace Clojure.Workspace.Explorer
{
	public class SolutionItem
	{
		private readonly string _path;
		private readonly SolutionItemType _itemType;

		public SolutionItem(string path, SolutionItemType itemType)
		{
			_itemType = itemType;
			_path = path;
		}

		public SolutionItemType ItemType
		{
			get { return _itemType; }
		}
		public string Path
		{
			get { return _path; }
		}
	}
}