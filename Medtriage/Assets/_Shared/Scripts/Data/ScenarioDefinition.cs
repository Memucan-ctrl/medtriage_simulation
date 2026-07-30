using System.Collections.Generic;
using UnityEngine;

namespace Medtriage.Shared.Data
{
    public enum GroundTruthRhythm { Shockable, NonShockable }

    /// <summary>Authored constants for one simulation scenario.</summary>
    [CreateAssetMenu(fileName = "ScenarioDefinition", menuName = "Medtriage/Scenario Definition")]
    public class ScenarioDefinition : ScriptableObject
    {
        [Header("Identity - must match TaskCatalog")]
        public string ScenarioId;
        public string DisplayName;

        [Header("Clinical ground truth - requires advisor review")]
        public GroundTruthRhythm GroundTruthRhythm = GroundTruthRhythm.Shockable;
        public string TrueReversibleCause;

        [Header("Pacing")]
        [Min(1)] public int MaxCycles = 4;
        [Min(1f)] public float EpinephrineIntervalSeconds = 180f;
        [Min(1f)] public float CycleDurationSeconds = 120f;

        [Header("Scoring")]
        public CategoryWeighting Weighting = new CategoryWeighting();
        public List<CriticalErrorRule> CriticalErrorCatalog = new List<CriticalErrorRule>();

        public string GetCriticalErrorDescription(string ruleId)
        {
            CriticalErrorRule rule = CriticalErrorCatalog.Find(item => item != null && item.Id == ruleId);
            return rule != null && !string.IsNullOrWhiteSpace(rule.Description) ? rule.Description : ruleId;
        }
    }
}
