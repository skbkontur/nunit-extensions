using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using NUnit.Framework.Interfaces;

namespace SkbKontur.NUnit.Middlewares
{
    public class CompositeSetup : ISetup
    {
        public CompositeSetup(List<SetUpAsync<TearDownAsync>> setups)
        {
            this.setups = setups;
        }

        private const string successfulSetupsKey = "SuccessfulSetups";

        public async Task SetUpAsync(ITest test)
        {
            foreach (var setup in setups)
            {
                var teardown = await setup(test).ConfigureAwait(false);
                test.Properties.Add(successfulSetupsKey, teardown);
            }
        }

        public async Task TearDownAsync(ITest test)
        {
            var exceptions = new List<Exception>();
            for (var i = test.Properties[successfulSetupsKey].Count - 1; i >= 0; i--)
            {
                try
                {
                    var teardown = (TearDownAsync)test.Properties[successfulSetupsKey][i];
                    await teardown().ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }
            }

            if (exceptions.Any())
            {
                throw new AggregateException($"TearDown for test {test.Name} failed", exceptions);
            }
        }

        private readonly List<SetUpAsync<TearDownAsync>> setups;
    }
}