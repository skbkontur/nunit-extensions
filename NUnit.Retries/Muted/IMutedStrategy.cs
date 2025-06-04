namespace SkbKontur.NUnit.Retries.Muted
{
    public interface IMutedStrategy
    {
        public string? Reason { get; }
        string? GetMuteValidationError();
    }
}