using System;

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace SkbKontur.NUnit.Retries.Gitlab
{
    public class RetryOnGitlabStrategy : IRetryStrategy
    {
        public RetryOnGitlabStrategy(int tryCount)
        {
            TryCount = tryCount;
        }

        public int TryCount { get; }

        public bool ShouldRetry(TestResult result)
        {
            return TestContextExtensions.IsOnGitlab() &&
                   (result.ResultState == ResultState.Failure ||
                    result.ResultState == ResultState.Error);
        }

        public void OnTestFailed(TestExecutionContext context, DateTimeOffset start)
        {
            context.WriteFailureForGitlab(start, TryCount);
        }
    }
}
