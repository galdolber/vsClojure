using Clojure.Code.Editing.BraceMatching;
using Clojure.Code.Parsing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clojure.Code.Tests.Editing.BraceMatching
{
	[TestClass]
	public class MatchingBraceFinderTests
	{
		private Tokenizer _tokenizer;
		private MatchingBraceFinder _finder;

		[TestInitialize]
		public void Initialize()
		{
			_tokenizer = new Tokenizer();
			_finder = new MatchingBraceFinder();
		}

		[TestMethod]
		public void ShouldNotFindAnyMatchingBracesWhenCursorIsNotTouchingAny()
		{
			var buffer = _tokenizer.Tokenize("(declare sym1)");
			MatchingBracePair pair = _finder.FindMatchingBraces(buffer, buffer.Length(), 10);
			Assert.IsNull(pair.Start);
			Assert.IsNull(pair.End);
		}

		[TestMethod]
		public void ShouldNotFindAnyMatchingBracesWhenCursorIsInWhitespaceAtEndOfText()
		{
			var buffer = _tokenizer.Tokenize("(declare sym1) ");
			MatchingBracePair pair = _finder.FindMatchingBraces(buffer, buffer.Length(), 10);
			Assert.IsNull(pair.Start);
			Assert.IsNull(pair.End);
		}

		[TestMethod]
		public void ShouldFindMatchingBraceWhenCursorIsRightBeforeStartOfListAtBeginningOfText()
		{
			var bufferText = "(declare sym1) ";
			var buffer = _tokenizer.Tokenize(bufferText);
			MatchingBracePair pair = _finder.FindMatchingBraces(buffer, buffer.Length(), bufferText.IndexOf("("));
			Assert.IsNotNull(pair.Start);
			Assert.IsNotNull(pair.End);
			Assert.AreEqual(bufferText.IndexOf("("), pair.Start.StartIndex);
			Assert.AreEqual(bufferText.IndexOf(")"), pair.End.StartIndex);
		}

		[TestMethod]
		public void ShouldFindMatchingBraceWhenCursorIsRightBeforeStartOfListNotAtBeginningOfText()
		{
			string bufferText = " (declare sym1) ";
			var buffer = _tokenizer.Tokenize(bufferText);
			MatchingBracePair pair = _finder.FindMatchingBraces(buffer, buffer.Length(), bufferText.IndexOf("("));
			Assert.IsNotNull(pair.Start);
			Assert.IsNotNull(pair.End);
			Assert.AreEqual(bufferText.IndexOf("("), pair.Start.StartIndex);
			Assert.AreEqual(bufferText.IndexOf(")"), pair.End.StartIndex);
		}

		[TestMethod]
		public void ShouldNotFindMatchingBracesWhenCursorIsJustRightOfStartOfList()
		{
			string bufferText = "(declare sym1) ";
			var buffer = _tokenizer.Tokenize(bufferText);
			MatchingBracePair pair = _finder.FindMatchingBraces(buffer, buffer.Length(), bufferText.IndexOf("d"));
			Assert.IsNull(pair.Start);
			Assert.IsNull(pair.End);
		}

		[TestMethod]
		public void ShouldNotFindMatchingBracesWhenCursorIsJustLeftOfEndOfList()
		{
			string bufferText = "(declare sym1) ";
			var buffer = _tokenizer.Tokenize(bufferText);
			MatchingBracePair pair = _finder.FindMatchingBraces(buffer, buffer.Length(), bufferText.IndexOf(")"));
			Assert.IsNull(pair.Start);
			Assert.IsNull(pair.End);
		}

		[TestMethod]
		public void ShouldFindMatchingBracesWhenCursorIsJustAfterLastListEnd()
		{
			string bufferText = "(declare sym1) ";
			var buffer = _tokenizer.Tokenize(bufferText);
			MatchingBracePair pair = _finder.FindMatchingBraces(buffer, buffer.Length(), bufferText.LastIndexOf(" "));
			Assert.IsNotNull(pair.Start);
			Assert.IsNotNull(pair.End);
		}

		[TestMethod]
		public void ShouldFindMatchingBracesWhenCursorIsJustAfterLastListEndAndAtEndOfText()
		{
			string bufferText = "(declare sym1)";
			var buffer = _tokenizer.Tokenize(bufferText);
			MatchingBracePair pair = _finder.FindMatchingBraces(buffer, buffer.Length(), bufferText.LastIndexOf(")") + 1);
			Assert.IsNotNull(pair.Start);
			Assert.IsNotNull(pair.End);
		}

		[TestMethod]
		public void ShouldNotFindMatchingBracesWhenCursorIsAfterLastCharacterNonBraceCharacterInText()
		{
			string bufferText = "(declare sym1";
			var buffer = _tokenizer.Tokenize(bufferText);
			MatchingBracePair pair = _finder.FindMatchingBraces(buffer, buffer.Length(), bufferText.LastIndexOf("1") + 1);
			Assert.IsNull(pair.Start);
			Assert.IsNull(pair.End);
		}

		[TestMethod]
		public void ShouldNotFindMatchingBracesWhenTextIsEmpty()
		{
			string bufferText = "";
			var buffer = _tokenizer.Tokenize(bufferText);
			MatchingBracePair pair = _finder.FindMatchingBraces(buffer, buffer.Length(), 0);
			Assert.IsNull(pair.Start);
			Assert.IsNull(pair.End);
		}
	}
}