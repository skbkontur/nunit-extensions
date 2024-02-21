using System;

using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace SkbKontur.NUnit.Retries
{
    public class RetryOnErrorStrategy : IRetryStrategy
    {
        public RetryOnErrorStrategy(int tryCount)
        {
            TryCount = tryCount;
        }

        public int TryCount { get; }

        public bool ShouldRetry(TestResult result)
        {
            return result.ResultState == ResultState.Failure || result.ResultState == ResultState.Error;
        }

        public void OnTestFailed(TestExecutionContext context, DateTimeOffset start)
        {
            var attempt = context.CurrentRepeatCount + 1;

            TestContext.Progress.WriteLine($"Test failed on attempt {attempt}/{TryCount}");
            TestContext.Progress.WriteLine(context.CurrentResult.Message);
            TestContext.Progress.WriteLine("Retrying...");
        }
    }
}