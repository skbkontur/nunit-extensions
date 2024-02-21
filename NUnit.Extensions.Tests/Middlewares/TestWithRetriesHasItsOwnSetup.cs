using System;

using FluentAssertions;

using NUnit.Framework;
using NUnit.Framework.Internal;

using SkbKontur.NUnit.Middlewares;

namespace SkbKontur.NUnit.Extensions.Tests.Middlewares
{
    public class DisposableCounter : IDisposable
    {
        public int InvocationsCount { get; set; }

        public void Dispose()
        {
            if (!disposed)
            {
                lock (sync)
                {
                    if (!disposed)
                    {
                        disposed = true;
                        return;
                    }
                }
            }

            throw new InvalidOperationException("Already disposed");
        }

        private readonly object sync = new();
        private bool disposed;
    }

    [Parallelizable(ParallelScope.Children)]
    public class TestWithRetriesHasItsOwnSetup : SimpleTestBase
    {
        [Test]
        [Retry(3)]
        public void TestRepeatCounterWithRetries()
        {
            var repeatCount = TestExecutionContext.CurrentContext.CurrentRepeatCount;

            var repeatCounter = SimpleTestContext.Current.Get<DisposableCounter>("repeat-counter");
            repeatCounter.Should().NotBeNull();
            repeatCounter.InvocationsCount.Should().Be(repeatCount);
            repeatCounter.InvocationsCount++;

            if (repeatCount is 0 or 1)
            {
                Assert.Fail("Third time's a Charm");
            }

            repeatCounter.InvocationsCount.Should().Be(repeatCount + 1);
        }

        [Test]
        [Retry(3)]
        public void TestCounterWithRetries()
        {
            var simpleCounter = SimpleTestContext.Current.Get<DisposableCounter>("simple-counter");
            simpleCounter.Should().NotBeNull();
            simpleCounter.InvocationsCount.Should().Be(0);
            simpleCounter.InvocationsCount++;

            if (TestExecutionContext.CurrentContext.CurrentRepeatCount is 0 or 1)
            {
                Assert.Fail("Third time's a Charm");
            }

            simpleCounter.InvocationsCount.Should().Be(1);
        }

        protected override void Configure(ISetupBuilder fixture, ISetupBuilder test)
        {
            test
                .Use(t =>
                    {
                        var counter = new DisposableCounter();
                        counter.InvocationsCount = TestExecutionContext.CurrentContext.CurrentRepeatCount;
                        t.Properties.Set("repeat-counter", counter);

                        return () => counter.Dispose();
                    })
                .Use(t =>
                    {
                        t.Properties.Set("simple-counter", new DisposableCounter());

                        return () => ((IDisposable)t.Properties.Get("simple-counter")).Dispose();
                    });
        }
    }
}