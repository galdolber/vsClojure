using Clojure.Base.CommandWindow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Clojure.Base.Tests.CommandWindow
{
	[TestClass]
	public class HistoryTests
	{
        [TestMethod]
        public void NextDoesNothingWhenNoItemsAreInHistory()
        {
            var historyEventListener = MockRepository.GenerateStrictMock<IHistoryEventListener>();
            var history = new History(historyEventListener);
            history.Next();
            historyEventListener.VerifyAllExpectations();
        }

		[TestMethod]
		public void NextSelectsFirstItemInHistoryWhenUserHasNotYetCycled()
		{
            var historyEventListener = MockRepository.GenerateMock<IHistoryEventListener>();
			var history = new History(historyEventListener);
			history.Submit("first-command");
			history.Next();
			historyEventListener.AssertWasCalled(l => l.HistoryItemSelected("first-command"));
		}

        [TestMethod]
        public void NextSelectsTheSubsequentItemInHistoryWhenMoreThanOneItemInHistory()
        {
            var historyEventListener = MockRepository.GenerateMock<IHistoryEventListener>();
            var history = new History(historyEventListener);

            using (historyEventListener.GetMockRepository().Ordered())
            {
                historyEventListener.Expect(l => l.HistoryItemSelected("third-command"));
                historyEventListener.Expect(l => l.HistoryItemSelected("second-command"));
                historyEventListener.Expect(l => l.HistoryItemSelected("first-command"));
            }

            historyEventListener.Replay();
            history.Submit("first-command");
            history.Submit("second-command");
            history.Submit("third-command");
            history.Next();
            history.Next();
            history.Next();
            historyEventListener.VerifyAllExpectations();
        }

        [TestMethod]
        public void NextCannotGoPastLastItemInHistory()
        {
            var historyEventListener = MockRepository.GenerateStrictMock<IHistoryEventListener>();
            var history = new History(historyEventListener);
            historyEventListener.Expect(l => l.HistoryItemSelected("second-command")).Repeat.Once();
            historyEventListener.Expect(l => l.HistoryItemSelected("first-command")).Repeat.Once();
            history.Submit("first-command");
            history.Submit("second-command");
            history.Next();
            history.Next();
            history.Next();
            historyEventListener.VerifyAllExpectations();
        }

        [TestMethod]
        public void PrevioustDoesNothingWhenNoItemsAreInHistory()
        {
            var historyEventListener = MockRepository.GenerateStrictMock<IHistoryEventListener>();
            var history = new History(historyEventListener);
            history.Previous();
            historyEventListener.VerifyAllExpectations();
        }

		[TestMethod]
		public void PreviousSelectsFirstItemInHistoryWhenOnSecondItem()
		{
            var historyEventListener = MockRepository.GenerateMock<IHistoryEventListener>();
			historyEventListener.Expect(a => a.HistoryItemSelected("second-command")).Repeat.Twice();

			var history = new History(historyEventListener);
			history.Submit("first-command");
			history.Submit("second-command");
			history.Next();
			history.Next();
			history.Previous();
			
			historyEventListener.VerifyAllExpectations();
		}

        [TestMethod]
        public void PreviousCanNotGoPastTheMostRecentCommandInHistory()
        {
            var historyEventListener = MockRepository.GenerateStrictMock<IHistoryEventListener>();

            using (historyEventListener.GetMockRepository().Ordered())
            {
                historyEventListener.Expect(l => l.HistoryItemSelected("second-command"));
                historyEventListener.Expect(l => l.HistoryItemSelected("first-command"));
                historyEventListener.Expect(l => l.HistoryItemSelected("second-command"));
            }

            historyEventListener.Replay();

            var history = new History(historyEventListener);
            history.Submit("first-command");
            history.Submit("second-command");
            history.Next();
            history.Next();
            history.Previous();
            history.Previous();
            history.Previous();

            historyEventListener.VerifyAllExpectations();
        }

        [TestMethod]
        public void SubmitDoesNotIncludeItemInHistoryWhenItMatchesTheMostRecentItem()
        {
            var historyEventListener = MockRepository.GenerateStrictMock<IHistoryEventListener>();

            using (historyEventListener.GetMockRepository().Ordered())
            {
                historyEventListener.Expect(l => l.HistoryItemSelected("second-command"));
                historyEventListener.Expect(l => l.HistoryItemSelected("first-command"));
            }

            historyEventListener.Replay();

            var history = new History(historyEventListener);
            history.Submit("first-command");
            history.Submit("second-command");
            history.Submit("second-command");
            history.Next();
            history.Next();

            historyEventListener.VerifyAllExpectations();
        }

        [TestMethod]
        public void SubmitResetsHistoryIndex()
        {
            var historyEventListener = MockRepository.GenerateStrictMock<IHistoryEventListener>();

            using (historyEventListener.GetMockRepository().Ordered())
            {
                historyEventListener.Expect(l => l.HistoryItemSelected("second-command"));
                historyEventListener.Expect(l => l.HistoryItemSelected("first-command"));
                historyEventListener.Expect(l => l.HistoryItemSelected("third-command"));
            }

            historyEventListener.Replay();

            var history = new History(historyEventListener);
            history.Submit("first-command");
            history.Submit("second-command");
            history.Next();
            history.Next();
            history.Submit("third-command");
            history.Next();

            historyEventListener.VerifyAllExpectations();
        }
	}
}
