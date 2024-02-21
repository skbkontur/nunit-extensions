using System;

namespace SkbKontur.NUnit.Retries.TeamCity
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
    public class RetryOnTeamCityAttribute : RetryAttribute
    {
        public RetryOnTeamCityAttribute(int tryCount)
            : base(new RetryOnTeamCityStrategy(tryCount))
        {
        }
    }
}