namespace SkbKontur.NUnit.Retries.Muted
{
    public interface IMutedStrategy
    {
        public string Reason { get; }
        public string? Error { get; }
        bool ShouldBeMuted();
    }
}