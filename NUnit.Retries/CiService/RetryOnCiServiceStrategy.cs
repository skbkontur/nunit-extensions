using System;

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace SkbKontur.NUnit.Retries.CiService
{
    public class RetryOnCiServiceStrategy : IRetryStrategy
    {
        public RetryOnCiServiceStrategy(int tryCount)
        {
            TryCount = tryCount;
        }

        public int TryCount { get; }

        public bool ShouldRetry(TestResult result)
        {
            return (CiServiceExtensions.GetCurrentService() == CiServiceExtensions.CiService.Gitlab ||
                    CiServiceExtensions.GetCurrentService() == CiServiceExtensions.CiService.TeamCity) &&
                   (result.ResultState == ResultState.Failure ||
                    result.ResultState == ResultState.Error ||
                    result.ResultState == ResultState.Warning);
        }

        public void OnTestFailed(TestExecutionContext context, DateTimeOffset start)
        {
            context.WriteFailure(start, TryCount);
        }
    }
}