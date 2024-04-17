using System.Threading.Tasks;

namespace SkbKontur.NUnit.Middlewares
{
    public static class SetupBuilderExtensions
    {
        public static ISetupBuilder Use(this ISetupBuilder builder, SetUp setup)
        {
            return builder.Use(test =>
                {
                    setup(test);

                    return Task.FromResult<TearDownAsync>(() => Task.CompletedTask);
                });
        }

        public static ISetupBuilder Use(this ISetupBuilder builder, SetUpAsync setup)
        {
            return builder.Use(async test =>
                {
                    await setup(test).ConfigureAwait(false);

                    return () => Task.CompletedTask;
                });
        }

        public static ISetupBuilder Use(this ISetupBuilder builder, SetUp<TearDown> setup)
        {
            return builder.Use(test =>
                {
                    var teardown = setup(test);

                    return Task.FromResult<TearDownAsync>(() =>
                        {
                            teardown();
                            return Task.CompletedTask;
                        });
                });
        }

        public static ISetupBuilder Use(this ISetupBuilder builder, SetUp<TearDownAsync> setup)
        {
            return builder.Use(test =>
                {
                    var teardown = setup(test);
                    return Task.FromResult(teardown);
                });
        }

        public static ISetupBuilder Use(this ISetupBuilder builder, SetUpAsync<TearDown> setup)
        {
            return builder.Use(async test =>
                {
                    var teardown = await setup(test).ConfigureAwait(false);

                    return () =>
                        {
                            teardown();
                            return Task.CompletedTask;
                        };
                });
        }

        public static ISetupBuilder UseSetup<TSetup>(this ISetupBuilder builder, params object[] args)
            where TSetup : ISetup
        {
            return builder.Use(async test =>
                {
                    var factory = test.TryGetFromThisOrParentContext<ISetupFactory>() ?? setupFactory;
                    var setup = factory.Create(typeof(TSetup), args);

                    await setup.SetUpAsync(test).ConfigureAwait(false);

                    return () => setup.TearDownAsync(test);
                });
        }

        private static readonly ISetupFactory setupFactory = new SetupFactory();
    }
}