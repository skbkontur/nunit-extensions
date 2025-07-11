namespace SkbKontur.NUnit.Retries.Common
{
    public static class ServiceMessageConstants
    {
        public static string ServiceMessageOpen()
        {
            var result = TestContextExtensions.IsOnGitlab() ? "##gitlab[" : "##teamcity[";
            return result;
        }
        public const string ServiceMessageClose = "]";
    }
}