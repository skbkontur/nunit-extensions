namespace SkbKontur.NUnit.Retries.CiService
{
    public static class ServiceMessageConstants
    {
        public const string ServiceMessageClose = "]";
        public static string ServiceMessageOpen = CiServiceExtensions.GetCurrentService().GetMessageStartConstants();
    }
}