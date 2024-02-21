using System;

namespace SkbKontur.NUnit.Retries
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
    public class RetryOnErrorAttribute : RetryAttribute
    {
        public RetryOnErrorAttribute(int tryCount)
            : base(new RetryOnErrorStrategy(tryCount))
        {
        }
    }
}