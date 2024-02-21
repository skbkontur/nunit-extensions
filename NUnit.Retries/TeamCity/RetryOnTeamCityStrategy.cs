using System;

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

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
            return TestContextExtensions.IsOnTeamCity() &&
                   (result.ResultState == ResultState.Failure ||
                    result.ResultState == ResultState.Error);
        }

        public void OnTestFailed(TestExecutionContext context, DateTimeOffset start)
        {
            context.WriteFailureForTeamCity(start, TryCount);
        }
    }
}