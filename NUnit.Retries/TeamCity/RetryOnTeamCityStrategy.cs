using System;

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using SkbKontur.NUnit.Retries.Common;

namespace SkbKontur.NUnit.Retries.TeamCity
{
    public class RetryOnTeamCityStrategy : IRetryStrategy
    {
        public RetryOnTeamCityStrategy(int tryCount)
        {
            TryCount = tryCount;
        }

        public int TryCount { get; }

        public bool ShouldRetry(TestResult result)
        {
            return CiServiceExtensions.GetCurrentService() == CiServiceExtensions.CiService.TeamCity &&
                   (result.ResultState == ResultState.Failure ||
                    result.ResultState == ResultState.Error);
        }

        public void OnTestFailed(TestExecutionContext context, DateTimeOffset start)
        {
            context.WriteFailure(start, TryCount);
        }
    }
}