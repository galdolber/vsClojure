using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Clojure.Code.Repl;
using Clojure.System.CommandWindow;
using Clojure.System.CommandWindow.Presentation;

namespace Clojure.VisualStudio.Repl.Presentation
{
	public class ReplTab : TabItem, IReplOutputListener, ICloseListener
	{
		private readonly List<ICloseListener> _listeners;
		private readonly CommandTextBox _commandWindow;
		private readonly IRepl _repl;

		public ReplTab(IRepl repl)
		{
			_repl = repl;
			_listeners = new List<ICloseListener>();

			var interactiveText = CreateInteractiveText();
			var closeButton = CreateCloseButton();

			Header = CreateHeaderPanel(CreateTabLabel(), closeButton);
			Content = CreateTextBoxGrid(interactiveText);

			_commandWindow = new CommandTextBox(interactiveText);
			repl.AddSubmitKeyHandlers(_commandWindow);
			repl.AddReplOutputListener(this);

			Loaded += (o, e) => repl.Start();
			closeButton.Click += (o, e) => DispatchCloseEvent();
		}

		public void AddCloseTabListener(ICloseListener listener)
		{
			_listeners.Add(listener);
		}

		public void ReplOutput(string text)
		{
			_commandWindow.Write(text);
		}

		public void OnTabClose()
		{
			_repl.Stop();
		}

		private void DispatchCloseEvent()
		{
			_listeners.ForEach(l => l.OnTabClose());
		}

		private static WrapPanel CreateHeaderPanel(Label replName, Button closeButton)
		{
			var headerPanel = new WrapPanel();
			headerPanel.Children.Add(replName);
			headerPanel.Children.Add(closeButton);
			return headerPanel;
		}

		private static Grid CreateTextBoxGrid(TextBox textBox)
		{
			var grid = new Grid();
			grid.Children.Add(textBox);
			return grid;
		}

		private static Label CreateTabLabel()
		{
			var name = new Label();
			name.Content = "Repl";
			name.Height = 19;
			name.HorizontalAlignment = HorizontalAlignment.Left;
			name.VerticalAlignment = VerticalAlignment.Top;
			name.VerticalContentAlignment = VerticalAlignment.Center;
			name.FontFamily = new FontFamily("Courier");
			name.FontSize = 12;
			name.Padding = new Thickness(0);
			name.Margin = new Thickness(0, 1, 0, 0);
			return name;
		}

		private Button CreateCloseButton()
		{
			var closeButton = new Button();
			closeButton.Content = "X";
			closeButton.Width = 20;
			closeButton.Height = 19;
			closeButton.FontFamily = new FontFamily("Courier");
			closeButton.FontSize = 12;
			closeButton.FontWeight = (FontWeight) new FontWeightConverter().ConvertFromString("Bold");
			closeButton.HorizontalAlignment = HorizontalAlignment.Right;
			closeButton.VerticalAlignment = VerticalAlignment.Top;
			closeButton.Style = (Style) closeButton.FindResource(ToolBar.ButtonStyleKey);
			closeButton.Margin = new Thickness(3, 0, 0, 0);
			return closeButton;
		}

		private static TextBox CreateInteractiveText()
		{
			var interactiveText = new TextBox();
			interactiveText.HorizontalAlignment = HorizontalAlignment.Stretch;
			interactiveText.VerticalAlignment = VerticalAlignment.Stretch;
			interactiveText.FontSize = 12;
			interactiveText.FontFamily = new FontFamily("Courier New");
			interactiveText.Margin = new Thickness(0, 0, 0, 0);
			interactiveText.IsEnabled = true;
			interactiveText.AcceptsReturn = true;
			interactiveText.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
			return interactiveText;
		}
	}
}