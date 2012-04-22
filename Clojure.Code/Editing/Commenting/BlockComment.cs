using System.Collections.Generic;

namespace Clojure.Code.Editing.Commenting
{
	public class BlockComment
	{
		public List<string> Execute(List<string> lines)
		{
			var commentedLines = new List<string>();
			foreach (var line in lines) commentedLines.Add(";" + line);
			return commentedLines;
		}
	}
}