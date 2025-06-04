using System;

namespace SkbKontur.NUnit.Retries.Muted
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class MutedAttribute : MutedBaseAttribute
    {
        public MutedAttribute(string reason, string until)
            : base(new MutedStrategy(reason, until))
        {
        }
    }
}