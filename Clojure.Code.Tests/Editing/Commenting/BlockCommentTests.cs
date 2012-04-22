using System.Collections.Generic;
using Clojure.Code.Editing.Commenting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Clojure.Code.Tests.Editing.Commenting
{
	[TestClass]
	public class BlockCommentTests
	{
		private List<string> _selectedLines;
		private BlockComment _blockComment;

		[TestInitialize]
		public void Initialize()
		{
			_selectedLines = new List<string>();
			_blockComment = new BlockComment();
		}

		[TestMethod]
		public void ShouldPutSemicolonInFrontOfEachLine()
		{
			_selectedLines.Add("line one");
			_selectedLines.Add("line two");
			_selectedLines.Add("line three");
			var result = _blockComment.Execute(_selectedLines);

			var expectedResult = new List<string>();
			expectedResult.Add(";line one");
			expectedResult.Add(";line two");
			expectedResult.Add(";line three");

			Assert.IsTrue(expectedResult.SequenceEqual(result));
		}

		[TestMethod]
		public void ShouldPutSemicolonOnBlankLines()
		{
			_selectedLines.Add("line one");
			_selectedLines.Add("");
			_selectedLines.Add("line three");
			var result = _blockComment.Execute(_selectedLines);

			var expectedResult = new List<string>();
			expectedResult.Add(";line one");
			expectedResult.Add(";");
			expectedResult.Add(";line three");

			Assert.IsTrue(expectedResult.SequenceEqual(result));
		}

		[TestMethod]
		public void ShouldPutSemicolonOnBlankLineIfItIsTheOnlyLine()
		{
			_selectedLines.Add("");
			var result = _blockComment.Execute(_selectedLines);
			var expectedResult = new List<string>();
			expectedResult.Add(";");

			Assert.IsTrue(expectedResult.SequenceEqual(result));
		}

		[TestMethod]
		public void ShouldDoNothingWhenNoLinesSelected()
		{
			var result = _blockComment.Execute(_selectedLines);
			var expectedResult = new List<string>();
			Assert.IsTrue(expectedResult.SequenceEqual(result));
		}
	}
}