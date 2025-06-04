using System;

namespace SkbKontur.NUnit.Retries.Muted
{
    public class MutedStrategy : IMutedStrategy
    {
        public MutedStrategy(string reason, string until, int maxDays = 30)
        {
            Reason = reason;
            Until = until;
            MaxDays = maxDays;
        }

        public bool ShouldBeMuted()
        {
            Error = null;
                
            if (!DateTime.TryParse(Until, out var parsed))
            {
                Error = $"Invalid 'until' format: {Until}";
                return false;
            }

            if (DateTime.UtcNow > parsed)
            {
                return false;
            }
            
            if ((parsed - DateTime.UtcNow).TotalDays > MaxDays)
            {
                Error = $"Muted until {parsed:u} exceeds max allowed period of {MaxDays} days.";
                return false;
            }

            return true;
        }

        public string Reason { get; }
        public string? Error { get; private set; }
        private string Until { get; }
        private int MaxDays { get; }
    }
}