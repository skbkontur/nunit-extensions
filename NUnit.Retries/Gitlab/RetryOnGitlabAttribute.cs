using System;

namespace SkbKontur.NUnit.Retries.Gitlab
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
    public class RetryOnGitlabAttribute : RetryAttribute
    {
        public RetryOnGitlabAttribute(int tryCount)
            : base(new RetryOnGitlabStrategy(tryCount))
        {
        }
    }
}