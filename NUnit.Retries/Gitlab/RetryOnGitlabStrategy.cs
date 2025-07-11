using System;

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using SkbKontur.NUnit.Retries.Common;

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
            return CiServiceExtensions.GetCurrentService() == CiServiceExtensions.CiService.Gitlab &&
                   (result.ResultState == ResultState.Failure ||
                    result.ResultState == ResultState.Error);
        }

        public void OnTestFailed(TestExecutionContext context, DateTimeOffset start)
        {
            context.WriteFailure(start, TryCount);
        }
    }
}