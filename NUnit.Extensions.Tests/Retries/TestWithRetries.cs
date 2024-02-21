using NUnit.Framework;
using NUnit.Framework.Internal;

using SkbKontur.NUnit.Retries;

namespace SkbKontur.NUnit.Extensions.Tests.Retries
{
    public class TestWithRetries
    {
        [Test]
        public void Test1()
        {
            if (TestExecutionContext.CurrentContext.CurrentRepeatCount is 0)
            {
                Assert.Fail("Second time's a Charm");
            }
        }

        [Test]
        [RetryOnError(3)]
        public void Test2()
        {
            if (TestExecutionContext.CurrentContext.CurrentRepeatCount is 0 or 1)
            {
                Assert.Fail("Third time's a Charm");
            }
        }
    }
}