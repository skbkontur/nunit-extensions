using NUnit.Framework;

using SkbKontur.NUnit.Retries;

namespace SkbKontur.NUnit.Extensions.Tests.Retries
{
    [SetUpFixture]
    [RetryOnError(2)]
    public class RetrySuite
    {
    }
}