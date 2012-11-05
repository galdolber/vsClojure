using System;

namespace Clojure.Code.Repl
{
	public interface IReplWriteCompleteDispatcher
	{
		void AddReplWriteCompleteListener(IReplWriteCompleteListener listener);
	}
}