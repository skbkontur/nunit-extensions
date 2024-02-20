using FluentAssertions;

using NUnit.Framework;

namespace SkbKontur.NUnit.Middlewares.Tests
{
    // sivukhin: (nit) some descriptions for test meaning will be very helpful (could be comment or maybe meaningful name)
    [Parallelizable(ParallelScope.Self)]
    public class FirstTestFixtureSetUpTest : SimpleTestBase
    {
        protected override void Configure(ISetupBuilder fixture, ISetupBuilder test)
        {
            fixture.Use(t => t.Properties.Set("item1", new object()));
        }

        [Test]
        public void Test()
        {
            SimpleTestContext.Current.ContainsKey("item1").Should().BeTrue();
        }
    }

    [Parallelizable(ParallelScope.Self)]
    public class SecondTestFixtureSetUpTest : SimpleTestBase
    {
        protected override void Configure(ISetupBuilder fixture, ISetupBuilder test)
        {
            fixture.Use(t => t.Properties.Set("item2", new object()));
        }

        [Test]
        public void Test()
        {
            SimpleTestContext.Current.ContainsKey("item2").Should().BeTrue();
        }
    }

    // sivukhin: this test looks strange as it always overwrite "item3" property with fresh new Counter()
    [Parallelizable(ParallelScope.Children)]
    public class ThirdTestFixtureSetUpTest : SimpleTestBase
    {
        protected override void Configure(ISetupBuilder fixture, ISetupBuilder test)
        {
            fixture.Use(t =>
                {
                    var item3 = t.Properties.Get("item3");
                    if (item3 != null)
                    {
                        ((Counter)item3).InvocationsCount++;
                    }
                    t.Properties.Set("item3", new Counter());
                });
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
            var item3 = SimpleTestContext.Current.Get("item3");
            item3.Should().NotBeNull();

            ((Counter)item3!).InvocationsCount.Should().Be(0);
        }
    }
}
