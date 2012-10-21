using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Clojure.System.CommandWindow;
using Clojure.System.CommandWindow.EventHandlers;

namespace Clojure.Code.Repl
{
	public static class ReplExtensions
	{
		public static string CreateScriptToLoadFilesIntoRepl(this IEnumerable<string> filesToLoad)
		{
			if (filesToLoad.Count() == 0) throw new Exception("No files to load.");

			var loadFileExpression = new StringBuilder("(map load-file '(");
			filesToLoad.ToList().ForEach(path => loadFileExpression.Append(" \"").Append(path.Replace("\\", "\\\\")).Append("\""));
			loadFileExpression.Append("))");

			return loadFileExpression.ToString();
		}

		public static IEnumerable<string> FindAllClojureFiles(this List<string> fileList)
		{
			return fileList.Where(p => p.ToLower().EndsWith(".clj"));
		}

		public static void WriteInvisiblyTo(this string expresion, IRepl repl)
		{
			repl.WriteInvisibly(expresion);
		}

		public static void LoadFilesInto(this List<string> fileList, IRepl repl)
		{
			repl.LoadFiles(fileList);
		}
	}
}