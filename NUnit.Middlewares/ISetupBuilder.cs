namespace SkbKontur.NUnit.Middlewares
{
    public interface ISetupBuilder
    {
        ISetupBuilder Use(SetUpAsync<TearDownAsync> setup);
        ISetup Build();
    }
}
