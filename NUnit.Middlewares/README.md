# NUnit.Middlewares

[![NuGet Status](https://img.shields.io/nuget/v/SkbKontur.NUnit.Middlewares.svg)](https://www.nuget.org/packages/SkbKontur.NUnit.Middlewares/)
[![Build status](https://github.com/skbkontur/nunit-extensions/actions/workflows/actions.yml/badge.svg)](https://github.com/skbkontur/nunit-extensions/actions)

Use middleware pattern to write tests in concise and comprehensive manner. And ditch test bases.

## Test setup middlewares

Inspired by ASP.NET Core [middlewares](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware), the main idea of test middlewares can be summarized by this image:

![nunit-middlewares](https://github.com/skbkontur/nunit-extensions/assets/5417867/9707428f-11ec-4353-ac96-7fdf70200a47)

Here we focus on *behaviours* that we want to add to our test rather than focusing on implementing test lifecycle methods provided by NUnit.

`suite`, `fixture` and `test` in the image above are just `ISetupBuilder` that can accept either raw setup functions or anything that implements simple `ISetup` interface:

![setup-builder](https://github.com/skbkontur/nunit-extensions/assets/5417867/e4adb7c6-2078-401e-9bac-539f89ffec54)

## Simple test base

To inject this new behaviour into our tests, we will use two simple base classes: `SimpleSuiteBase` and `SimpleTestBase`, our tests from first image can be set up as follows:

```csharp
[SetUpFixture]
public class PlaywrightSuite : SimpleSuiteBase
{
  protected override void Configure(ISetupBuilder suite)
  {
    suite
      .UseHostingEnvironment()
      .UseSimpleContainer()
      .UseSetup<PlaywrightSetup>();
  }
}

public class BusinessObjectsSearchTests : SimpleTestBase
{
  [Injected] // Injected from container by `InitializeInjectedSetup`
  private readonly IUserProvider userProvider;

  protected override void Configure(ISetupBuilder fixture, ISetupBuilder test)
  {
    fixture
      .UseSetup<InitializeInjectedSetup>();

    test
      .UseSetup<BrowserPerTestSetup>();
  }

  [Test]
  public async Task BasicTest()
  {
    // every test gets its own browser, thus making tests easily parallelizable
    var browser = SimpleTestContext.Current.Get<Browser>();

    await browser.LoginAsync(userProvider.DefaultUser);
    await browser.Page.GotoAsync("https://google.com");
    await browser.Page.GetByTitle("Search").FillAsync("nunit");
  }
}
```

## Composition over inheritance

With the power of C#'s extension methods, we can use composition of setups instead of relying on inheritance. For example, here's how setup for our container can be written:

```csharp
public static class SetupExtensions
{
  public static ISetupBuilder UseSimpleContainer(
    this ISetupBuilder builder,
    Action<ContainerBuilder>? configure = null)
  {
    return builder
      // our container needs hosting environment, hence we should always set it up,
      // but if it was already set up earlier, we will use existing environment
      .UseSetup(new HostingEnvironmentSetup(setupOnlyIfNotExists: true))
      .UseSetup(new SimpleContainerSetup(configure));
  }
}

public class SimpleContainerSetup : ISetup
{
  private readonly Action<ContainerBuilder>? configure;

  public SimpleContainerSetup(Action<ContainerBuilder>? configure)
  {
    this.configure = configure;
  }

  public Task SetUpAsync(ITest test)
  {
    var environment = test.GetFromThisOrParentContext<IHostingEnvironment>();
    var container = ContainerFactory.NewContainer(environment, configure);
    test.Properties.Set(container); // save container to current test context

    return Task.CompletedTask;
  }

  public Task TearDownAsync(ITest test)
  {
    var container = test.Properties.Get<IContainer>();
    container.Dispose();

    return Task.CompletedTask;
  }
}
```

Using these building blocks, we can move all the complexity of setups to separate, smaller code pieces (`ISetup`s), and make setups more reusable in the process.

## Simple test context

In our `BasicTest` above we used `SimpleTestContext.Current.Get<Browser>()` to get browser that we set up in `BrowserPerTestSetup`. Also, in `SimpleContainerSetup` we used `GetFromThisOrParentContext` method that can access items that previous setups have set up. How does it work? Good news is that we can use built-in NUnit features to build such test context.

`TestExecutionContext.CurrentContext.CurrentTest` - current test, implements `ITest`

How do we get container/browser from suite context in our test? Every test has property `IPropertyBag Properties`.

Tests in NUnit are represented by a tree-like structure, and `ITest` has access to parent through `ITest Parent` property. Parent for test method is test fixture, parent for fixture is suite and so on.

That means we can search for *context item* of interest in parent, if not found - in parent's parent

To ensure everything is working as intended, parent's *context item*s should be used as **readonly**

In our example from first image, test context will look something like this:

![test-context](https://github.com/skbkontur/nunit-extensions/assets/5417867/c70b41d6-5f3f-485a-9e9d-7616b3797232)

Both `SimpleTestContext` and `GetFromThisOrParentContext` are just `ITest` wrappers that search for context value in `ITest`'s `Properties` recursively

## Why are test bases a problem?

To make a point, let's try to rewrite test above without our testing machinery.

Let's start with `BusinessObjectsSearchTests.cs`:

```csharp
public class BusinessObjectsSearchTests : PlaywrightTestBase
{
  [Injected]
  private readonly IUserProvider userProvider;

  [Test]
  public async Task BasicTest()
  {
    // every test gets its own browser, thus making tests easily parallelizable
    await using var browser = await BrowserPerTest();

    await browser.LoginAsync(userProvider.DefaultUser);
    await browser.Page.GotoAsync("https://google.com");
    await browser.Page.GetByTitle("Search").FillAsync("nunit");
  }
}
```

So far so good, notice that we moved `BrowserPerTestSetup` into the test itself. A neat trick that would be more difficult if we had more per test instances to set up.

`PlaywrightTestBase` looks simple enough. But we had to make our Browser `IAsyncDisposable`:

```csharp
public class PlaywrightTestBase : SimpleContainerTestBase
{
  protected IPlaywright playwright;
  protected IBrowser browser;

  [OneTimeSetUp]
  public async Task SetUpPlaywright()
  {
    playwright = await Playwright.CreateAsync();
    browser = await playwright.Chromium.LaunchAsync()
  }

  [OneTimeTearDown]
  public async Task TearDownPlaywright()
  {
    await browser.DisposeAsync().ConfigureAwait(false);
    playwright.Dispose();
  }

  protected async Task<Browser> BrowserPerTest()
  {
    var page = await browser.NewPageAsync();
    return new Browser(page); // now Browser is responsible for disposing of page
  }
}
```

How deep does this rabbit hole go? Let's dive into `SimpleContainerTestBase`:

```csharp
public class SimpleContainerTestBase
{
  protected IContainer container;

  [OneTimeSetUp]
  public void SetUpContainer()
  {
    var environment = HostingEnvironment.Create();
    container = ContainerFactory.NewContainer(environment, ConfigureContainer);
    ContainerFactory.InitializeInjectedFields(container, this);
  }

  [OneTimeTearDown]
  public void TearDownContainer()
  {
    container.Dispose();
  }

  protected virtual void ConfigureContainer(ContainerBuilder builder)
  {
  }
}
```

Now it doesn't look that bad. What did we miss? Quite a few things:
- it was harder to setup items per test and keep tests parallelizable
- to shorten chain of inheritance, we tightly integrated setup of HostingEnvironment and Container and forgot to dispose of hosting environment
- we set up container and hosting environment for each test, before we only set it up once. Refactoring it can be a PITA, especially if `container` or `browser` field is referenced in our tests. On the other hand, when using nunit-middlewares, we can refactor such case by moving two lines of code.
- what if many of our test fixtures need an organization to work with? would we make `class OrganizationTestBase : PlaywrightTestBase`? and if we need an organization, but don't need browser?
- our example is rather simple, in more complex cases, our test bases can quickly become a nightmare to debug and extend

Excellent example of a complex case is playwright integration with nunit in official [Playwright.NUnit](https://github.com/microsoft/playwright-dotnet/tree/main/src/Playwright.NUnit) package:
- it has `PageTest` that inherits `ContextTest` that inherits `BrowserTest` that inherits `PlaywrightTest` that inherits `WorkerAwareTest`... whoa