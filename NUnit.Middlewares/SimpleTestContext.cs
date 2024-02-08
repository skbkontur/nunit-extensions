using System.Collections;

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

        public object? Get(string key) => test.Get(key);
        public bool ContainsKey(string key) => test.ContainsKey(key);
        public IList? this[string key] => test.List(key);

        private readonly ITest test;
    }
}