using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Clojure.System.IO.Keyboard;

namespace Clojure.System.CommandWindow
{
	public class CommandTextBox : ICommandWindow
	{
		private readonly TextBox _textBox;
		private readonly List<IKeyEventHandler> _keyEventHandlers;
		private int _promptPosition;

		public CommandTextBox(TextBox textBox, List<IKeyEventHandler> keyEventHandlers)
		{
			_textBox = textBox;
			_keyEventHandlers = keyEventHandlers;
			_textBox.PreviewKeyDown += previewKeyDown;
			this.AddDefaultKeyHandlers(keyEventHandlers);
		}

		private void previewKeyDown(object sender,  KeyEventArgs args)
		{
			var examiner = new KeyboardExaminer();

			var snapshot = new CommandWindowUserEvent(
				_textBox.CaretIndex,
				_promptPosition,
				args.Key,
				examiner.IsShiftDown(),
				examiner.ControlIsDown(),
				_textBox.Text.Substring(_promptPosition));

			var handlers = _keyEventHandlers.FindAll(h => h.CanHandle(snapshot));
			args.Handled = handlers.Any();
			handlers.ForEach(h => h.Handle(snapshot));
		}

		public void ReplaceCurrentExpressionWith(string value)
		{
			_textBox.Text = _textBox.Text.Remove(_promptPosition, _textBox.Text.Length - _promptPosition);
			_textBox.AppendText(value);
			_textBox.CaretIndex = _textBox.Text.Length;
		}

		public void MoveCursorToStartOfPrompt()
		{
			_textBox.CaretIndex = _promptPosition;
		}

		public void UpdateSelectionToIncludeTextFromCursorPositionToPrompt()
		{
			_textBox.SelectionStart = _promptPosition;
		}

		public void EraseSelection()
		{
			_textBox.Text = _textBox.Text.Remove(_textBox.SelectionStart, _textBox.SelectionLength);
			_textBox.CaretIndex = _promptPosition;
		}

		public void Write(string output)
		{
			_textBox.Dispatcher.Invoke(
				DispatcherPriority.Normal,
				new DispatcherOperationCallback(
					delegate
					{
						_textBox.AppendText(output);
						_textBox.ScrollToEnd();
						_promptPosition = _textBox.Text.Length;
						return null;
					}), null);
		}

		public void WriteLine()
		{
			Write("\r\n");
		}
	}
}
