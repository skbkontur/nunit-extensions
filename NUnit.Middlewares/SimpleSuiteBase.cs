using System.Threading.Tasks;

using NUnit.Framework;
using NUnit.Framework.Internal;

namespace SkbKontur.NUnit.Middlewares
{
    public abstract class SimpleSuiteBase
    {
        [OneTimeSetUp]
        public Task _OneTimeSetUp()
        {
            var builder = new SetupBuilder();

            Configure(builder);

            suiteSetup = builder.Build();

            return suiteSetup.SetUpAsync(TestExecutionContext.CurrentContext.CurrentTest);
        }

        [OneTimeTearDown]
        public Task _OneTimeTearDown()
        {
            return suiteSetup?.TearDownAsync(TestExecutionContext.CurrentContext.CurrentTest) ?? Task.CompletedTask;
        }

        protected abstract void Configure(ISetupBuilder suite);

        private ISetup? suiteSetup;
    }
}