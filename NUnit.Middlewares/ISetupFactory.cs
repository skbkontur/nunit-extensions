using System;

namespace SkbKontur.NUnit.Middlewares
{
    public interface ISetupFactory
    {
        ISetup Create(Type setupType, object[] args);
    }
}