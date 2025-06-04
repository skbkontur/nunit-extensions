using System;

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;

namespace SkbKontur.NUnit.Retries.Muted
{
    public class MutedCommand : DelegatingTestCommand
    {
        public MutedCommand(TestCommand innerCommand, IMutedStrategy strategy)
            : base(innerCommand)
        {
            this.strategy = strategy;
        }

        public override TestResult Execute(TestExecutionContext context)
        {
            context.CurrentResult = context.CurrentTest.MakeTestResult();

            try
            {
                context.CurrentResult = innerCommand.Execute(context);
            }
            catch (Exception ex)
            {
                context.CurrentResult.RecordException(ex);
            }

            if (context.CurrentResult.ResultState == ResultState.Failure || context.CurrentResult.ResultState == ResultState.Error)
            {
                var validationError = strategy.GetMuteValidationError();
                var testErrorPattern = $"Test error message:{context.CurrentResult.Message}, stackTrace: {context.CurrentResult.StackTrace}";
                if (validationError == null)
                {
                    context.CurrentResult.SetResult(ResultState.Warning, $"[Muted] Reason: {strategy.Reason ?? "unspecified"}.\n{testErrorPattern}");
                }
                else
                {
                    context.CurrentResult.SetResult(ResultState.Error, $"Validation error: {validationError}.\n{testErrorPattern}");
                }
            }

            return context.CurrentResult;
        }

        private readonly IMutedStrategy strategy;
    }
}