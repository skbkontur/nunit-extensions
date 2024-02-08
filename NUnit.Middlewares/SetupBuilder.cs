using System.Collections.Generic;

namespace SkbKontur.NUnit.Middlewares
{
    public class SetupBuilder : ISetupBuilder
    {
        public ISetupBuilder Use(SetUpAsync<TearDownAsync> setup)
        {
            setups.Add(setup);
            return this;
        }

        public ISetup Build()
        {
            return new CompositeSetup(setups);
        }

        private readonly List<SetUpAsync<TearDownAsync>> setups = new();
    }
}