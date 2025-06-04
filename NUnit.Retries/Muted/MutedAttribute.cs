using System;

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Commands;

namespace SkbKontur.NUnit.Retries.Muted
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MutedAttribute : Attribute, IWrapTestMethod
    {
        public MutedAttribute(string reason, string until)
        {
            strategy = new MutedStrategy(reason, until);
        }

        public MutedAttribute(IMutedStrategy strategy)
        {
            this.strategy = strategy;
        }

        public TestCommand Wrap(TestCommand command)
        {
            return new MutedCommand(command, strategy);
        }

        private readonly IMutedStrategy strategy;
    }
}