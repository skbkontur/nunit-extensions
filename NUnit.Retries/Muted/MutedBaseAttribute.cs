using System;
using System.Linq;

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;

namespace SkbKontur.NUnit.Retries.Muted
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class MutedBaseAttribute : Attribute, IWrapTestMethod, IApplyToContext
    {
        protected MutedBaseAttribute(IMutedStrategy strategy)
        {
           this.strategy = strategy;
        }

        public TestCommand Wrap(TestCommand command)
        {
            return new MutedCommand(command, strategy);
        }

        public void ApplyToContext(TestExecutionContext context)
        {
            ApplyToTestRecursively(context.CurrentTest);
        }

        private void ApplyToTestRecursively(Test test)
        {
            if (test is TestMethod method)
            {
                method.Method = new CustomAttributeMethodWrapper(method.Method, this);
                return;
            }

            foreach (var child in test.Tests.OfType<Test>())
            {
                ApplyToTestRecursively(child);
            }
        }

        private readonly IMutedStrategy strategy;
    }
}