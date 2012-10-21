using System.Collections.Generic;
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
			return ArrowKeys.Contains(key);
		}

		public static readonly List<Key> ArrowKeys = new List<Key>() { Key.Up, Key.Down, Key.Left, Key.Right };
	}
}