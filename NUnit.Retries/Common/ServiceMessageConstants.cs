namespace SkbKontur.NUnit.Retries.Common
{
    public static class ServiceMessageConstants
    {
        public const string ServiceMessageClose = "]";
        public static string ServiceMessageOpen = CiServiceExtensions.GetCurrentService().GetMessageStartConstants();
    }
}