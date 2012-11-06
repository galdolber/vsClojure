using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Clojure.System.CommandWindow;

namespace Clojure.Code.Repl
{
	public interface IRepl : IReplWriteRequestListener, ISubmitCommandListener
	{
		void AddReplWriteCompleteListener(IReplWriteCompleteListener listener);
		void AddReplOutputListener(IReplOutputListener listener);
		void Start();
		void Stop();
	}
}
