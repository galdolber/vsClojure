using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Clojure.Code.Repl;

namespace Clojure.VisualStudio.Repl
{
	public class ReplEventRouter : IReplWriteRequestListener
	{
		public ReplEventRouter()
		{
		}

		public void Write(string data)
		{
			throw new NotImplementedException();
		}
	}
}
