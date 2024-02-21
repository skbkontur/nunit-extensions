using System;

using NUnit.Framework;
using NUnit.Framework.Internal;

using SkbKontur.NUnit.Retries;

namespace SkbKontur.NUnit.Extensions.Tests.Retries
{
    [RetryOnError(3)]
    public class FixtureWithRetries
    {
        [Test]
        public void Test1()
        {
            if (TestExecutionContext.CurrentContext.CurrentRepeatCount is 0 or 1)
            {
                Assert.Fail("Third time's a Charm");
            }
        }

        [Test]
        [RetryOnError(4)]
        public void Test2()
        {
            if (TestExecutionContext.CurrentContext.CurrentRepeatCount is 0 or 1 or 2)
            {
                throw new Exception("Forth time's a Charm, now with exception");
            }
        }

        [Test]
        [NoRetry]
        public void Test3()
        {
            Assert.Pass();
        }
    }
}