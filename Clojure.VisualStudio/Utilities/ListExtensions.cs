using System.Collections.Generic;

namespace Clojure.VisualStudio.Utilities
{
	public static class ListExtensions
	{
		public static List<T> SingletonAsList<T>(this T singleton)
		{
			return new List<T>() { singleton };
		}
	}
}
