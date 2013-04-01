using System.Runtime.InteropServices;
using System.Windows.Controls;
using Clojure.Workspace.Repl;
using Clojure.Workspace.Repl.Presentation;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Clojure.VisualStudio.Workspace.Repl
{
	[Guid("8C5C7302-ECC8-435D-AAFE-D0E5A0A02FE9")]
	public class ReplToolWindow : ToolWindowPane, IReplWriteCompleteListener, IReplPortfolioListener
	{
		public ReplToolWindow()
		{
			Caption = "Repl Manager";
			BitmapResourceID = 301;
			BitmapIndex = 1;
            base.Content = new ReplTabControl();
		}

		public void OnReplWriteComplete()
		{
			var replToolWindowFrame = (IVsWindowFrame) this.Frame;
			replToolWindowFrame.ShowNoActivate();
		}

		public void ReplAdded(IRepl repl)
		{
			var replToolWindowFrame = (IVsWindowFrame) this.Frame;
			replToolWindowFrame.Show();
			repl.AddReplWriteCompleteListener(this);
		}
	}
}