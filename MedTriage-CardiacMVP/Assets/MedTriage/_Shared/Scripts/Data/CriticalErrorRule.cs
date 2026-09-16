using System;

namespace MedTriage.Shared.Data
{
    public enum CriticalErrorId
    {
        NoCompressions30s,
        ShockNonShockable,
        NoClearCall,
        PrematureTermination
    }

    [Serializable]
    public class CriticalErrorRule
    {
        public CriticalErrorId Id;
        public ScoreCategory Category;
        public float PenaltyPoints = 25f;
        public bool FailsSession;
    }
}
