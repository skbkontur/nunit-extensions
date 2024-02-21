using System;

using NUnit.Framework.Internal;

namespace SkbKontur.NUnit.Retries
{
    public interface IRetryStrategy
    {
        public int TryCount { get; }
        public bool ShouldRetry(TestResult result);
        public void OnTestFailed(TestExecutionContext context, DateTimeOffset start);
    }
}