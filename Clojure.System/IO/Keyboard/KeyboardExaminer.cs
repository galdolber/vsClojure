using System.Windows.Input;

namespace Clojure.System.IO.Keyboard
{
	public class KeyboardExaminer
	{
		public bool IsShiftDown()
		{
			return global::System.Windows.Input.Keyboard.IsKeyDown(Key.LeftShift) | global::System.Windows.Input.Keyboard.IsKeyDown(Key.RightShift);
		}

		public bool ControlIsDown()
		{
			return global::System.Windows.Input.Keyboard.IsKeyDown(Key.LeftCtrl) | global::System.Windows.Input.Keyboard.IsKeyDown(Key.RightCtrl);
		}

		public bool IsArrowKey(Key key)
		{
			return key == Key.Up || key == Key.Down || key == Key.Left || key == Key.Right;
		}
	}
}