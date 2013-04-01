using System.Windows.Input;

namespace Clojure.Base.CommandWindow
{
	public class CommandWindowUserEvent
	{
		private readonly int _cursorPosition;
		private readonly int _promptPosition;
		private readonly Key _keyPressed;
		private readonly bool _shiftDown;
		private readonly bool _controlDown;
		private readonly string _expression;

		public CommandWindowUserEvent(int cursorPosition, int promptPosition, Key keyPressed, bool shiftDown, bool controlDown, string expression)
		{
			_cursorPosition = cursorPosition;
			_promptPosition = promptPosition;
			_keyPressed = keyPressed;
			_shiftDown = shiftDown;
			_controlDown = controlDown;
			_expression = expression;
		}

		public string Expression
		{
			get { return _expression; }
		}

		public bool ControlDown
		{
			get { return _controlDown; }
		}

		public bool ShiftDown
		{
			get { return _shiftDown; }
		}

		public Key KeyPressed
		{
			get { return _keyPressed; }
		}

		public bool IsCursortAtOrAfterPrompt()
		{
			return IsCursorAfterPrompt() || IsCursorAtPrompt();
		}

		public bool IsCursorAfterPrompt()
		{
			return _cursorPosition > _promptPosition;
		}

		public bool IsCursorAtPrompt()
		{
			return _cursorPosition == _promptPosition;
		}
	}
}