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
            if (!DateTime.TryParse(until, out this.until))
            {
                throw new ArgumentException($"Invalid 'until' format: {until}");
            }

            const int maxDays = 30;
            if ((this.until - DateTime.UtcNow).TotalDays > maxDays)
            {
                throw new ArgumentException($"Muted until {this.until:u} exceeds max allowed period of {maxDays} days.");
            }
        }

        public TestCommand Wrap(TestCommand command)
        {
            return new MutedCommand(command, reason, until);
        }

        private readonly string reason;
        private readonly DateTime until;
    }
}