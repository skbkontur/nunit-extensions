using System;

namespace SkbKontur.NUnit.Middlewares
{
    public class SetupFactory : ISetupFactory
    {
        public ISetup Create(Type setupType, object[] args)
        {
            return (ISetup)Activator.CreateInstance(setupType, args);
        }
    }
}