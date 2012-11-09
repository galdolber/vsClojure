using System.Collections.Generic;

namespace Clojure.Code.Editing.TextBuffer
{
	public interface ITextBufferAdapter
	{
		string GetText(int startPosition);
		int Length { get; }
	}
}