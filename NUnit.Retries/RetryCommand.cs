using System;
using System.Collections.Generic;
using System.Text;

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
            var failedAttempts = new List<string>();
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
                    if (failedAttempts.Count > 0 && CiServiceExtensions.GetCurrentService() == CiServiceExtensions.CiService.Gitlab)
                    {
                        var attempts = failedAttempts.Count + 1;
                        context.CurrentTest.Properties.Set("Retries.Attempts", attempts);
                        result.SetResult(ResultState.Warning, FormatRetriedMessage(attempts, failedAttempts));
                    }
                    break;
                }

                failedAttempts.Add(FormatFailedAttempt(failedAttempts.Count + 1, context.CurrentResult));

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

        private string FormatRetriedMessage(int attempts, List<string> failedAttempts)
        {
            var message = new StringBuilder();
            message.Append($"[Retried] Passed on attempt {attempts}/{strategy.TryCount}.");

            foreach (var failedAttempt in failedAttempts)
            {
                message.AppendLine();
                message.Append(failedAttempt);
            }

            return message.ToString();
        }

        private static string FormatFailedAttempt(int attempt, TestResult result)
        {
            return $"Attempt {attempt} failed: {result.Message}, stackTrace: {result.StackTrace}";
        }
        
        private readonly IRetryStrategy strategy;
    }
}
