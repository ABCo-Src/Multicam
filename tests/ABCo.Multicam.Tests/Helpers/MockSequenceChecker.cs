using Moq;
using System.Linq.Expressions;

namespace ABCo.Multicam.Tests.Helpers
{
    public static class MockSequenceChecker
    {
        public class SequenceAssert
        {
            public int Progress;
            public int ExpectedEnd;

            public SequenceAssert(int expectedEnd) => ExpectedEnd = expectedEnd;
            public void Verify() => Assert.AreEqual(ExpectedEnd, Progress, "Sequence not executed fully.");
        }

        public static SequenceAssert SetupSequenceTracker<T>(this Mock<T> a, params Expression<Action<T>>[] methods) where T : class
        {
            var progress = new SequenceAssert(methods.Length);
            
            // Setup sequence
            for (int i = 0; i < methods.Length; i++)
            {
                int val = i;
                a.When(() => progress.Progress == val).Setup(methods[i]).Callback(() => progress.Progress++);
            }

            return progress;
        }
    }
}
