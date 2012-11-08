using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Clojure.Code.Parsing;

namespace Clojure.Workspace.Repl
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

		private static string ConvertToClojureNamespaceExpression(this string namespaceName)
		{
			return "(in-ns '" + namespaceName + ")";
		}

		public static void LoadFiles(this IReplWriteRequestListener repl, List<string> fileList)
		{
			repl.Write(fileList.FindAllClojureFiles().CreateScriptToLoadFilesIntoRepl());
		}

		public static void ChangeNamespace(this IReplWriteRequestListener repl, LinkedList<Token> newNamespace)
		{
			var namespaceParser = new NamespaceParser(NamespaceParser.NamespaceSymbols);
			repl.Write(namespaceParser.Execute(newNamespace).ConvertToClojureNamespaceExpression());
		}
	}
}