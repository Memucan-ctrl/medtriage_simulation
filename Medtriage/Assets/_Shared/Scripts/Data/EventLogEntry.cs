using System;

namespace Medtriage.Shared.Data
{
    /// <summary>A single auditable scenario event used by deterministic scoring.</summary>
    [Serializable]
    public class EventLogEntry
    {
        public string EventName;
        public string Category;
        public float TimestampSeconds;
        public string ExpectedValue;
        public string ActualValue;
        public bool Correct;
    }
}
