using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Clojure.Base.CommandWindow;
using Clojure.Base.CommandWindow.Presentation;

namespace Clojure.Workspace.Repl.Presentation
{
    public partial class ReplTab : TabItem, IReplOutputListener, ICloseListener
    {
        private readonly List<ICloseListener> _listeners;
        private readonly CommandTextBox _interactiveWindow;
        private readonly IRepl _repl;

        public ReplTab()
        {
            InitializeComponent();
        }

        public ReplTab(IRepl repl) : this()
        {
            _repl = repl;
            _listeners = new List<ICloseListener>();

            _interactiveWindow = new CommandTextBox(InteractiveText);
            repl.AddSubmitKeyHandlers(_interactiveWindow);
            repl.AddReplOutputListener(this);

            Loaded += (o, e) => repl.Start();
            CloseButton.Click += (o, e) => DispatchCloseEvent();
        }

        public void AddCloseTabListener(ICloseListener listener)
        {
            _listeners.Add(listener);
        }

        private void DispatchCloseEvent()
        {
            _listeners.ForEach(l => l.OnTabClose());
        }

        public void ReplOutput(string text)
        {
            _interactiveWindow.Write(text);
        }

        public void OnTabClose()
        {
            _repl.Stop();
        }
    }
}
