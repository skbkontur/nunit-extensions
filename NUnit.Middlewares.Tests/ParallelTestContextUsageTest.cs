using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace SkbKontur.NUnit.Middlewares.Tests
{
    public class Counter
    {
        public int InvocationsCount { get; set; }
    }

    public class TestInvocationCounterSetup : ISetup
    {
        public Task SetUpAsync(ITest test)
        {
            test.Properties.Set(new Counter());
            return Task.CompletedTask;
        }

        public Task TearDownAsync(ITest test)
        {
            return Task.CompletedTask;
        }
    }

    [Parallelizable(ParallelScope.Children)]
    public class ParallelTestContextUsageTest : SimpleTestBase
    {
        protected override void Configure(ISetupBuilder fixture, ISetupBuilder test)
        {
            test.UseSetup<TestInvocationCounterSetup>();
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        [TestCase(7)]
        [TestCase(8)]
        [TestCase(9)]
        [TestCase(10)]
        public void Test(int n)
        {
            TestInvocationCount();
        }

        [Test]
        public void Test1()
        {
            TestInvocationCount();
        }

        [Test]
        public void Test2()
        {
            TestInvocationCount();
        }

        [TestCaseSource(nameof(testCases))]
        public void TestSource(int i)
        {
            TestInvocationCount();
        }

        private static void TestInvocationCount()
        {
            var counter = SimpleTestContext.Current.Get<Counter>();
            counter.Should().NotBeNull();

            counter.InvocationsCount++;
            counter.InvocationsCount.Should().Be(1);
        }

        private static readonly TestCaseData[] testCases = Enumerable.Range(0, 100).Select(x => new TestCaseData(x)).ToArray();
    }
}