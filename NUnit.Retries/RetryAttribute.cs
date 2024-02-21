using System;
using System.Linq;

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;

namespace SkbKontur.NUnit.Retries
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
    public class RetryAttribute : Attribute, IRepeatTest, IApplyToContext
    {
        protected RetryAttribute(IRetryStrategy strategy)
        {
            this.strategy = strategy;
        }

        public TestCommand Wrap(TestCommand command)
        {
            return new RetryCommand(command, strategy);
        }

        public void ApplyToContext(TestExecutionContext context)
        {
            ApplyToTestRecursively(context.CurrentTest, overwrite : true);
        }

        private void ApplyToTestRecursively(Test test, bool overwrite)
        {
            if (test.GetCustomAttributes<NoRetryAttribute>(true).Any()
                || (!overwrite && test.GetCustomAttributes<RetryAttribute>(true).Any()))
            {
                return;
            }

            if (test is TestMethod method)
            {
                method.Method = new CustomAttributeMethodWrapper(method.Method, this);
                return;
            }

            foreach (var child in test.Tests.OfType<Test>())
            {
                ApplyToTestRecursively(child, overwrite : false);
            }
        }

        private readonly IRetryStrategy strategy;
    }
}