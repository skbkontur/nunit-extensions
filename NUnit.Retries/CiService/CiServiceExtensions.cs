using System;

namespace SkbKontur.NUnit.Retries.CiService
{
    public static class CiServiceExtensions
    {
        public enum CiService
        {
            Gitlab,
            TeamCity,
            Unknown
        }

        public static CiService GetCurrentService()
        {
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TEAMCITY_VERSION")))
            {
                return CiService.TeamCity;
            }
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GITLAB_CI")))
            {
                return CiService.Gitlab;
            }
            return CiService.Unknown;
        }
    }
}