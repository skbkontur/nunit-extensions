using System;


namespace SkbKontur.NUnit.Retries.CiService
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
    public class RetryOnCiServiceAttribute : RetryAttribute
    {
        public RetryOnCiServiceAttribute(int tryCount)
            : base(new RetryOnCiServiceStrategy(tryCount))
        {
        }
    }
}