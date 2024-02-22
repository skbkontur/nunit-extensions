using System.Threading.Tasks;

using NUnit.Framework;
using NUnit.Framework.Internal;

namespace SkbKontur.NUnit.Middlewares
{
    public abstract class SimpleTestBase
    {
        [OneTimeSetUp]
        public Task _OneTimeSetUp()
        {
            var fixture = new SetupBuilder();
            var test = new SetupBuilder();

            Configure(fixture, test);

            fixtureSetup = fixture.Build();
            testSetup = test.Build();

            return fixtureSetup.SetUpAsync(TestExecutionContext.CurrentContext.CurrentTest);
        }

        [OneTimeTearDown]
        public Task _OneTimeTearDown()
        {
            return fixtureSetup?.TearDownAsync(TestExecutionContext.CurrentContext.CurrentTest) ?? Task.CompletedTask;
        }

        [SetUp]
        public Task _SetUp()
        {
            return testSetup?.SetUpAsync(TestExecutionContext.CurrentContext.CurrentTest) ?? Task.CompletedTask;
        }

        [TearDown]
        public Task _TearDown()
        {
            return testSetup?.TearDownAsync(TestExecutionContext.CurrentContext.CurrentTest) ?? Task.CompletedTask;
        }

        protected abstract void Configure(ISetupBuilder fixture, ISetupBuilder test);

        private ISetup? fixtureSetup;
        private ISetup? testSetup;
    }
}