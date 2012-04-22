using Clojure.Code.Editing.Indenting;
using Clojure.Code.Parsing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clojure.Code.Tests.Editing.Indenting
{
	[TestClass]
	public class ClojureSmartIndentTests
	{
		private Tokenizer _tokenizer;
		private ClojureSmartIndent _clojureSmartIndent;
		private int _indentAmount;

		[TestInitialize]
		public void Initialize()
		{
			_tokenizer = new Tokenizer();
			_clojureSmartIndent = new ClojureSmartIndent();
			_indentAmount = 4;
		}

		[TestMethod]
		public void ShouldNotIndentEmptyBuffer()
		{
			Assert.AreEqual(0, _clojureSmartIndent.GetDesiredIndentation(_tokenizer.Tokenize(""), 0, _indentAmount));
		}

		[TestMethod]
		public void ShouldIndentOpenListWithNoFollowingTokensByIndentAmount()
		{
			Assert.AreEqual(4, _clojureSmartIndent.GetDesiredIndentation(_tokenizer.Tokenize("(\r\n"), 3, _indentAmount));
		}

		[TestMethod]
		public void ShouldIndentOpenListWithFollowingTokensByIndentAmount()
		{
			string input = "(asdf asdf 123\r\n";
			Assert.AreEqual(4, _clojureSmartIndent.GetDesiredIndentation(_tokenizer.Tokenize(input), input.IndexOf("\n") + 1, _indentAmount));
		}

		[TestMethod]
		public void ShouldIndentOpenListInsideAnotherListByTheIndentAmountPlusOne()
		{
			string input = "((\r\n";
			Assert.AreEqual(5, _clojureSmartIndent.GetDesiredIndentation(_tokenizer.Tokenize(input), input.IndexOf("\n") + 1, _indentAmount));
		}

		[TestMethod]
		public void ShouldIndentByIndentAmountWhenListContainsMultipleElements()
		{
			string input = "(asdf asdf\r\n";
			Assert.AreEqual(4, _clojureSmartIndent.GetDesiredIndentation(_tokenizer.Tokenize(input), input.IndexOf("\n") + 1, _indentAmount));
		}

		[TestMethod]
		public void ShouldIndentByIndentAmountWhenListContainsMultipleAndHasIndentElements()
		{
			string input = "(asdf (fdas\r\n))";
			Assert.AreEqual(input.LastIndexOf("(") + 4, _clojureSmartIndent.GetDesiredIndentation(_tokenizer.Tokenize(input), input.IndexOf("\n") + 1, _indentAmount));
		}

		[TestMethod]
		public void ShouldIndentByOneAfterTheOpeningBraceForVectors()
		{
			string input = "(asdf [\r\n]";
			Assert.AreEqual(input.LastIndexOf("[") + 1, _clojureSmartIndent.GetDesiredIndentation(_tokenizer.Tokenize(input), input.IndexOf("\n") + 1, _indentAmount));
		}

		[TestMethod]
		public void DropsExistingLineDownWhileMaintainingIndentAndIndentsTheCorrectAmountForNewLine()
		{
			string input = "(ns program (:gen-class))\n\n(defn -main [& args] (println \"Hello world\"))";
			Assert.AreEqual(4, _clojureSmartIndent.GetDesiredIndentation(_tokenizer.Tokenize(input), input.IndexOf("(println") - 1, _indentAmount));
		}
	}
}
