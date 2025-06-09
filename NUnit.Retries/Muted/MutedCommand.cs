using System;

using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;

namespace SkbKontur.NUnit.Retries.Muted
{
    public class MutedCommand : DelegatingTestCommand
    {
        public MutedCommand(TestCommand innerCommand, string reason, DateTime until)
            : base(innerCommand)
        {
            this.reason = reason;
            this.until = until;
        }

        public override TestResult Execute(TestExecutionContext context)
        {
            context.CurrentResult = context.CurrentTest.MakeTestResult();
            if (until < DateTime.UtcNow)
            {
                TestContext.Progress.WriteLine($"Muted until {until:u} is already in the past");
                return innerCommand.Execute(context);
            }

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
                context.CurrentResult.SetResult(ResultState.Warning, $"[Muted] Reason: {reason}.\nTest error message:{context.CurrentResult.Message}, stackTrace: {context.CurrentResult.StackTrace}");
            }

            return context.CurrentResult;
        }

        private readonly string reason;
        private readonly DateTime until;
    }
}