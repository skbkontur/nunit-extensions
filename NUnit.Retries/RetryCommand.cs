using System;

using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;

namespace SkbKontur.NUnit.Retries
{
    public class RetryCommand : DelegatingTestCommand
    {
        public RetryCommand(TestCommand innerCommand, IRetryStrategy strategy)
            : base(innerCommand)
        {
            this.strategy = strategy;
        }

        public override TestResult Execute(TestExecutionContext context)
        {
            var count = strategy.TryCount;

            while (count-- > 0)
            {
                var start = DateTimeOffset.UtcNow;
                try
                {
                    context.CurrentResult = innerCommand.Execute(context);
                }
                catch (Exception ex)
                {
                    context.CurrentResult ??= context.CurrentTest.MakeTestResult();
                    context.CurrentResult.RecordException(ex);
                }

                if (count <= 0 || !strategy.ShouldRetry(context.CurrentResult))
                {
                    break;
                }

                strategy.OnTestFailed(context, start);

                context.CurrentResult = context.CurrentTest.MakeTestResult();
                context.CurrentRepeatCount++;
            }

            return context.CurrentResult;
        }

        private readonly IRetryStrategy strategy;
    }
}