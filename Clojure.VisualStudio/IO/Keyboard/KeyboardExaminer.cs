using System.Windows.Input;

namespace Clojure.VisualStudio.IO.Keyboard
{
	public class KeyboardExaminer
	{
		public bool IsShiftDown()
		{
			return System.Windows.Input.Keyboard.IsKeyDown(Key.LeftShift) | System.Windows.Input.Keyboard.IsKeyDown(Key.RightShift);
		}

		public bool ControlIsDown()
		{
			return System.Windows.Input.Keyboard.IsKeyDown(Key.LeftCtrl) | System.Windows.Input.Keyboard.IsKeyDown(Key.RightCtrl);
		}

		public bool IsArrowKey(Key key)
		{
			return key == Key.Up || key == Key.Down || key == Key.Left || key == Key.Right;
		}
	}
}