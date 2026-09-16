using System;

namespace MedTriage.Shared.Data
{
    public enum SimEventType
    {
        RecognitionConfirmed,
        CodeCalled,
        PulseCheck,
        CompressionStream,
        CompressionInterruption,
        MonitorAttached,
        VentilationRatioAdherence,
        AdvancedAirwayPlaced,
        RoleDelegated,
        ClosedLoopConfirmed,
        RhythmClassified,
        ShockDelivered,
        PostShockCprResumed,
        PrematureRecheckAttempt,
        MedicationGiven,
        ReversibleCauseConsidered,
        ReversibleCauseIdentified,
        CycleCompleted,
        CompressorRotation,
        RoscAchieved,
        PostArrestActionSelected,
        TerminationDecision
    }

    [Serializable]
    public class EventLogEntry
    {
        public SimEventType EventType;
        public ScoreCategory Category;
        public float TimestampSeconds;
        public float Score;
        public float Weight = 1f;
        public bool IsCritical;
    }
}
