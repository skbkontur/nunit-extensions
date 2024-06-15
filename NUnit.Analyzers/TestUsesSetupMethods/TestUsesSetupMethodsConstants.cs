namespace NUnit.Analyzers.TestUsesSetupMethods
{
    internal class TestUsesSetupMethodsConstants
    {
        internal const string TestUsesSetupMethodsTitle = "Test uses setup methods";
        internal const string TestUsesSetupMethodsMessage = "Setup methods are considered harmful";
        internal const string TestUsesSetupMethodsDescription = "If you require a similar object or state for your tests, prefer a helper method than using Setup and Teardown attributes.";
        internal const string TestUsesSetupMethodsUri = "https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices#prefer-helper-methods-to-setup-and-teardown";
    }
}