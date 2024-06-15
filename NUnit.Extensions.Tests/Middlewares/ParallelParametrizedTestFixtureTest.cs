using System;
using System.Linq;

using FluentAssertions;

using NUnit.Framework;

using SkbKontur.NUnit.Middlewares;

namespace SkbKontur.NUnit.Extensions.Tests.Middlewares
{
    [TestFixture(-1)]
    [TestFixture(-2)]
    [TestFixture(-3)]
    [TestFixture(-4)]
    [TestFixture(-5)]
    [TestFixture(-6)]
    [TestFixture(-7)]
    [TestFixture(-8)]
    [TestFixture(-9)]
    [TestFixture(-10)]
    [TestFixtureSource(nameof(testCases))]
    [Parallelizable(ParallelScope.Self)]
    public class ParallelParametrizedTestFixtureTest : SimpleTestBase
    {
        public ParallelParametrizedTestFixtureTest(int i, int i2)
        {
            this.i = i;
            this.i1 = 1;
            E += () => {};
        }

        private event Action E;
        private const int i3 = 0;

        protected override void Configure(ISetupBuilder fixture, ISetupBuilder test)
        {
            i2 = 3;
            fixture
                .UseSetup<TestInvocationCounterSetup>()
                .Use(t => t.GetFromThisOrParentContext<Counter>().InvocationsCount += i);
        }

        [SetUp]
        public void F()
        {
        }

        [Test]
        public void Test()
        {
            E();
            Console.WriteLine(i1);
            Console.WriteLine(i2);
            Console.WriteLine(i3);
            var counter = SimpleTestContext.Current.Get<Counter>();
            counter.Should().NotBeNull();

            counter.InvocationsCount.Should().Be(i);
        }

        private readonly int i1;
        private int i2;

        private readonly int i;

        private static readonly TestFixtureData[] testCases = Enumerable.Range(0, 100).Select(x => new TestFixtureData(x)).ToArray();
    }
}