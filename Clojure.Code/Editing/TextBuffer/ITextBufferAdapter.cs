using System.Collections.Generic;

namespace Clojure.Code.Editing.TextBuffer
{
	public interface ITextBufferAdapter
	{
		string GetText(int startPosition);
		int Length { get; }
		void SetText(string text);
		List<string> GetSelectedLines();
		void ReplaceSelectedLines(List<string> newLines);
	}
}