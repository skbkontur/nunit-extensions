# NUnit.Retries

[![NuGet Status](https://img.shields.io/nuget/v/SkbKontur.NUnit.Retries.svg)](https://www.nuget.org/packages/SkbKontur.NUnit.Retries/)
[![Build status](https://github.com/skbkontur/nunit-extensions/actions/workflows/actions.yml/badge.svg)](https://github.com/skbkontur/nunit-extensions/actions)

Couple of helpful attributes for test retries:
- `RetryOnErrorAttribute` is like NUnit's own `RetryAttribute`, but it can be applied to whole Fixture/Suite/Assembly, and supports retry after exceptions in test, not only assertion failures
- On top of that, `RetryOnTeamCityAttribute` also supports TeamCity's [test retry](https://www.jetbrains.com/help/teamcity/2022.10/build-failure-conditions.html#test-retry) feature
- `NoRetryAttribute` for disabling retries

Attributes can be overriden on any level, e.g. 
- MyAssembly.dll: `[RetryOnError(2)]`
- MySuite.cs: `[NoRetry]`
- MyTestFixture.cs: `[RetryOnTeamCity(3)]`
- MyTestMethod(): `[RetryOnError(4)]`

This means we have two retries on assembly level in `MyAssembly.dll`, but no retries in `MySuite`,
if `MyTestFixture` is also in `MySuite`, previous attributes are overriden by `RetryOnTeamCity`,
and method `MyTestMethod` in `MyTestFixture` is retried 4 times.