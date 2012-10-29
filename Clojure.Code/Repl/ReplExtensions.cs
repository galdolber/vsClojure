using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Clojure.Code.Parsing;
using Clojure.System.CommandWindow;
using Clojure.System.CommandWindow.EventHandlers;

namespace Clojure.Code.Repl
{
	public static class ReplExtensions
	{
        private static string CreateScriptToLoadFilesIntoRepl(this IEnumerable<string> filesToLoad)
		{
			if (filesToLoad.Count() == 0) throw new Exception("No files to load.");

			var loadFileExpression = new StringBuilder("(map load-file '(");
			filesToLoad.ToList().ForEach(path => loadFileExpression.Append(" \"").Append(path.Replace("\\", "\\\\")).Append("\""));
			loadFileExpression.Append("))");

			return loadFileExpression.ToString();
		}

		private static IEnumerable<string> FindAllClojureFiles(this List<string> fileList)
		{
			return fileList.Where(p => p.ToLower().EndsWith(".clj"));
		}

        private static string ConvertToClojureNamespaceExpression(this string namespaceName)
        {
            return "(in-ns '" + namespaceName + ")";
        }

        public static void LoadFiles(this IRepl repl, List<string> fileList)
        {
			repl.Write(fileList.FindAllClojureFiles().CreateScriptToLoadFilesIntoRepl());
        }

        public static void ChangeNamespace(this IRepl repl, LinkedList<Token> newNamespace)
        {
			var namespaceParser = new NamespaceParser(NamespaceParser.NamespaceSymbols);
			repl.Write(namespaceParser.Execute(newNamespace).ConvertToClojureNamespaceExpression());
        }
	}
}