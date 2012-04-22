using System.Collections.Generic;
using System.Linq;
using Clojure.Code.Editing.Commenting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clojure.Code.Tests.Editing.Commenting
{
	[TestClass]
	public class BlockUncommentTests
	{
		private List<string> _selectedLines;
		private BlockUncomment _blockUncomment;

		[TestInitialize]
		public void Initialize()
		{
			_selectedLines = new List<string>();
			_blockUncomment = new BlockUncomment();
		}

		[TestMethod]
		public void ShouldRemoveSemicolonFromEachLine()
		{
			_selectedLines.Add(";line one");
			_selectedLines.Add(";line two");
			_selectedLines.Add(";line three");
			var result = _blockUncomment.Execute(_selectedLines);

			var expectedResult = new List<string>();

			expectedResult.Add("line one");
			expectedResult.Add("line two");
			expectedResult.Add("line three");

			Assert.IsTrue(expectedResult.SequenceEqual(result));
		}

		[TestMethod]
		public void ShouldRemoveSemicolonThatHasLeadingWhitespace()
		{
			_selectedLines.Add("    ;   line two");
			var result = _blockUncomment.Execute(_selectedLines);

			var expectedResult = new List<string>();
			expectedResult.Add("       line two");
			Assert.IsTrue(expectedResult.SequenceEqual(result));
		}

		[TestMethod]
		public void ShouldRemoveOnlyOneSemicolonIfMoreThanOneIsPresent()
		{
			_selectedLines.Add(";;line two");
			var result = _blockUncomment.Execute(_selectedLines);

			var expectedResult = new List<string>();
			expectedResult.Add(";line two");
			Assert.IsTrue(expectedResult.SequenceEqual(result));
		}

		[TestMethod]
		public void ShouldDoNothingToALineThatDoesNotBeginWithASemicolon()
		{
			_selectedLines.Add("line two");
			var result = _blockUncomment.Execute(_selectedLines);

			var expectedResult = new List<string>();
			expectedResult.Add("line two");
			Assert.IsTrue(expectedResult.SequenceEqual(result));
		}

		[TestMethod]
		public void ShouldDoNothingToALineThatEndsWithAComment()
		{
			_selectedLines.Add("line two ;asdf");
			var result = _blockUncomment.Execute(_selectedLines);

			var expectedResult = new List<string>();
			expectedResult.Add("line two ;asdf");
			Assert.IsTrue(expectedResult.SequenceEqual(result));
		}
	}
}