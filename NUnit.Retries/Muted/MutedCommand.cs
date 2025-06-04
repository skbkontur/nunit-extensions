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
                if (strategy.ShouldBeMuted())
                {
                    context.CurrentResult.SetResult(ResultState.Warning, $"[Muted] Reason: {strategy.Reason} — {ex.Message ?? ex.GetType().Name}");
                }
                else if (strategy.Error != null)
                {
                    context.CurrentResult.SetResult(ResultState.Error, strategy.Error);
                }
                else
                {
                    context.CurrentResult.RecordException(ex);
                }
            }

            return context.CurrentResult;
        }

        private readonly IMutedStrategy strategy;
    }
}