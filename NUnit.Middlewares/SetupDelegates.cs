using System.Threading.Tasks;

using NUnit.Framework.Interfaces;

namespace SkbKontur.NUnit.Middlewares
{
    public delegate void SetUp(ITest test);

    public delegate TTearDown SetUp<out TTearDown>(ITest test);

    public delegate Task SetUpAsync(ITest test);

    public delegate Task<TTearDown> SetUpAsync<TTearDown>(ITest test);

    public delegate void TearDown();

    public delegate Task TearDownAsync();
}