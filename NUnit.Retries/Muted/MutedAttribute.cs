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
            this.reason = reason;
            this.until = until;
        }

        public TestCommand Wrap(TestCommand command)
        {
            return new MutedCommand(command, reason, until, 30);
        }

        private readonly string reason;
        private readonly string until;
    }
}