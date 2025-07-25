using System;

using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;
using NUnit.Framework.Interfaces;
using SkbKontur.NUnit.Retries.CiService;

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
            bool hasFailuresBeforeSuccess = false;
            TestResult result = null;

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

                if (context.CurrentResult.ResultState == ResultState.Success)
                {
                    result = context.CurrentResult;
                    if (hasFailuresBeforeSuccess && CiServiceExtensions.GetCurrentService() == CiServiceExtensions.CiService.Gitlab)
                    {
                        result.SetResult(
                            ResultState.Warning
                        );
                    }
                    break;
                }

                hasFailuresBeforeSuccess = true;

                if (count <= 0 || !strategy.ShouldRetry(context.CurrentResult))
                {
                    result = context.CurrentResult;
                    break;
                }

                strategy.OnTestFailed(context, start);
                context.CurrentResult = context.CurrentTest.MakeTestResult();
                context.CurrentRepeatCount++;
            }

            return result;
        }

        private readonly IRetryStrategy strategy;
    }
}