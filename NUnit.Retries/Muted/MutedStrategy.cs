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

        public string? GetMuteValidationError()
        {
            if (!DateTime.TryParse(Until, out var parsed))
            {
                return $"Invalid 'until' format: {Until}";
            }

            var now = DateTime.UtcNow;

            if (parsed < now)
            {
                return $"Muted until {parsed:u} is already in the past.";
            }

            if ((parsed - now).TotalDays > MaxDays)
            {
                return $"Muted until {parsed:u} exceeds max allowed period of {MaxDays} days.";
            }

            return null;
        }

        public string Reason { get; }
        private string Until { get; }
        private int MaxDays { get; }
    }
}