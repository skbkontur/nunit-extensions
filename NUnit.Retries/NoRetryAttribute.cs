using System;

namespace SkbKontur.NUnit.Retries
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class NoRetryAttribute : Attribute
    {
    }
}