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
    public partial class ReplTabControl : TabControl, IReplPortfolioListener
    {
        private List<IReplActivationListener> _activationListeners;

        public ReplTabControl()
        {
            InitializeComponent();
            _activationListeners = new List<IReplActivationListener>();
        }

        public void ReplAdded(IRepl repl)
        {
            var tab = new ReplTab(repl);
            tab.AddCloseTabListener(new TabCloseListener(this, tab));
            Items.Add(tab);
            SelectionChanged += (o, e) => TryDispatchActivationEvent(tab, repl);
            SelectedItem = tab;
        }

        public void AddReplActivationListener(IReplActivationListener listener)
        {
            _activationListeners.Add(listener);
        }

        private void TryDispatchActivationEvent(ReplTab tab, IRepl repl)
        {
            if (tab.IsSelected) _activationListeners.ForEach(l => l.ReplActivated(repl));
        }

        private class TabCloseListener : ICloseListener
        {
            private readonly ReplTabControl _parentControl;
            private readonly ReplTab _tab;

            public TabCloseListener(ReplTabControl parentControl, ReplTab tab)
            {
                _parentControl = parentControl;
                _tab = tab;
            }

            public void OnTabClose()
            {
                _parentControl.Items.Remove(_tab);
            }
        }
    }
}
