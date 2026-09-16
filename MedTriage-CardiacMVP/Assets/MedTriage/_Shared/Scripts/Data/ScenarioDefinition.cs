using System.Collections.Generic;
using UnityEngine;

namespace MedTriage.Shared.Data
{
    public enum RhythmType
    {
        Shockable,
        NonShockable
    }

    public enum ScenarioPhase
    {
        Recognition,
        BasicLifeSupport,
        TeamRolesAndRhythm,
        ShockableBranch,
        NonShockableBranch,
        Cycles,
        Outcome
    }

    public class ScenarioDefinition : ScriptableObject
    {
        public string ScenarioId;
        public string DisplayName;
        public RhythmType GroundTruthRhythm = RhythmType.Shockable;
        public string TrueReversibleCause;
        public int MaxCycles = 4;
        public float EpinephrineIntervalSeconds = 180f;
        public float CycleDurationSeconds = 120f;
        public List<CategoryWeighting> Weights = new List<CategoryWeighting>();
        public List<CriticalErrorRule> CriticalErrors = new List<CriticalErrorRule>();
    }
}
