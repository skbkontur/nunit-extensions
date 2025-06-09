using System;

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;

namespace SkbKontur.NUnit.Retries.Muted
{
    public class MutedCommand : DelegatingTestCommand
    {
        public MutedCommand(TestCommand innerCommand, string? reason, string until, int maxDays)
            : base(innerCommand)
        {
            this.reason = reason;
            if (!DateTime.TryParse(until, out this.until))
            {
                throw new ArgumentException($"Invalid 'until' format: {until}");
            }
            this.maxDays = maxDays;
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
                var validationError = GetMuteValidationError();
                var testErrorPattern = $"Test error message:{context.CurrentResult.Message}, stackTrace: {context.CurrentResult.StackTrace}";
                if (validationError == null)
                {
                    context.CurrentResult.SetResult(ResultState.Warning, $"[Muted] Reason: {reason ?? "unspecified"}.\n{testErrorPattern}");
                }
                else
                {
                    context.CurrentResult.SetResult(ResultState.Error, $"Validation error: {validationError}.\n{testErrorPattern}");
                }
            }

            return context.CurrentResult;
        }

        private string? GetMuteValidationError()
        {
            var now = DateTime.UtcNow;

            if (until < now)
            {
                return $"Muted until {until:u} is already in the past.";
            }

            if ((until - now).TotalDays > maxDays)
            {
                return $"Muted until {until:u} exceeds max allowed period of {maxDays} days.";
            }

            return null;
        }

        private readonly string? reason;
        private readonly DateTime until;
        private readonly int maxDays;
    }
}