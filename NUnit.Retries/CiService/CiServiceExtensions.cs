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

        public static string GetMessageStartConstants(this CiService service)
        {
            return service switch
                {
                    CiService.Gitlab => "##gitlab[",
                    CiService.TeamCity => "#teamcity[",
                    _ => throw new NotSupportedException("Not supported CI service")
                };
        }
    }
}