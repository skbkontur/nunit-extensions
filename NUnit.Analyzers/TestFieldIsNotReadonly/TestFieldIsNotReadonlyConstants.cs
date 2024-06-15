namespace NUnit.Analyzers.TestFieldIsNotReadonly
{
    internal class TestFieldIsNotReadonlyConstants
    {
        internal const string TestFieldIsNotReadonlyTitle = "The field in test class is not readonly";
        internal const string TestFieldIsNotReadonlyMessage = "Fields in test classes should be readonly";
        internal const string TestFieldIsNotReadonlyDescription = "A test fixture should not contain any modifiable shared state to simplify tests parallel execution.";
    }
}