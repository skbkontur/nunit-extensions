using System.Collections.Generic;

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace SkbKontur.NUnit.Middlewares
{
    public class SimpleTestContext
    {
        private SimpleTestContext(ITest test)
        {
            this.test = test;
        }

        public static SimpleTestContext Current => new(TestExecutionContext.CurrentContext.CurrentTest);

        public object? Get(string key) => test.GetRecursive(key);
        public bool ContainsKey(string key) => test.ContainsKeyRecursive(key);
        public IReadOnlyList<object>? this[string key] => (IReadOnlyList<object>?)test.ListRecursive(key);

        private readonly ITest test;
    }
}