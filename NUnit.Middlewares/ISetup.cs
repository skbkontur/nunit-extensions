using System.Threading.Tasks;

using NUnit.Framework.Interfaces;

namespace SkbKontur.NUnit.Middlewares
{
    public interface ISetup
    {
        Task SetUpAsync(ITest test);
        Task TearDownAsync(ITest test);
    }
}