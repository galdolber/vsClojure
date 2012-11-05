using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Clojure.Code.Repl;

namespace Clojure.VisualStudio.Repl.Commands
{
	public class LoadSelectionCommand : IMenuCommandListener
	{
		private readonly IReplWriteRequestListener _requestListener;

		public LoadSelectionCommand(IReplWriteRequestListener requestListener)
		{
			_requestListener = requestListener;
		}

		public void OnMenuCommandClick()
		{
			
		}
	}
}
