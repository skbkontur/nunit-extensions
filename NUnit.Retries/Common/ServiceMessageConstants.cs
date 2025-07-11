namespace SkbKontur.NUnit.Retries.Common
{
    public static class ServiceMessageConstants
    {
        public static string ServiceMessageOpen = CiServiceExtensions.GetCurrentService().GetMessageStartConstants();
        public const string ServiceMessageClose = "]";
    }
}