using System.Threading.Tasks;

using NUnit.Framework.Interfaces;

namespace SkbKontur.NUnit.Middlewares
{
    // sivukhin: should we add native support for cancellation in the core delegates signatures (pass CancellationToken in SetUpAsync / TearDownAsync?)
    public delegate void SetUp(ITest test);

    public delegate TTearDown SetUp<out TTearDown>(ITest test);

    public delegate Task SetUpAsync(ITest test);

    public delegate Task<TTearDown> SetUpAsync<TTearDown>(ITest test);

    public delegate void TearDown();

    public delegate Task TearDownAsync();
}
