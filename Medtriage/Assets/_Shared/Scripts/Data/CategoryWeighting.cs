using System;
using UnityEngine;

namespace Medtriage.Shared.Data
{
    /// <summary>Relative score weights. ScoringCalculator normalizes assessed categories.</summary>
    [Serializable]
    public class CategoryWeighting
    {
        [Min(0f)] public float Efficiency = 0.25f;
        [Min(0f)] public float ProtocolAdherence = 0.30f;
        [Min(0f)] public float TechnicalExecution = 0.20f;
        [Min(0f)] public float TeamCommunication = 0.15f;
        [Min(0f)] public float DecisionMaking = 0.10f;

        public float GetWeight(string category)
        {
            if (category == ScoreCategories.Efficiency) return Efficiency;
            if (category == ScoreCategories.ProtocolAdherence) return ProtocolAdherence;
            if (category == ScoreCategories.TechnicalExecution) return TechnicalExecution;
            if (category == ScoreCategories.TeamCommunication) return TeamCommunication;
            if (category == ScoreCategories.DecisionMaking) return DecisionMaking;
            return 0f;
        }
    }
}
