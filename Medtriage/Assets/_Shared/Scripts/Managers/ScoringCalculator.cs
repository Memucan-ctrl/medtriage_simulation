using System.Collections.Generic;
using System.Linq;
using Medtriage.Shared.Data;

namespace Medtriage.Shared.Managers
{
    /// <summary>Deterministic scoring. Unassessed categories are omitted, not awarded free points.</summary>
    public static class ScoringCalculator
    {
        private static readonly string[] AllCategories =
        {
            ScoreCategories.Efficiency,
            ScoreCategories.ProtocolAdherence,
            ScoreCategories.TechnicalExecution,
            ScoreCategories.TeamCommunication,
            ScoreCategories.DecisionMaking
        };

        public static List<CategoryScore> ComputeCategoryScores(IReadOnlyList<EventLogEntry> eventLog)
        {
            var scores = new List<CategoryScore>();
            if (eventLog == null) return scores;

            foreach (string category in AllCategories)
            {
                List<EventLogEntry> assessed = eventLog.Where(item => item != null && item.Category == category).ToList();
                if (assessed.Count == 0) continue;

                float score = 100f * assessed.Count(item => item.Correct) / assessed.Count;
                scores.Add(new CategoryScore { Category = category, Score = score });
            }

            return scores;
        }

        public static float ComputeComposite(IReadOnlyList<CategoryScore> categoryScores, CategoryWeighting weighting)
        {
            if (categoryScores == null || categoryScores.Count == 0 || weighting == null) return 0f;

            float weightedTotal = 0f;
            float assessedWeight = 0f;
            foreach (CategoryScore categoryScore in categoryScores)
            {
                float weight = weighting.GetWeight(categoryScore.Category);
                if (weight <= 0f) continue;
                weightedTotal += categoryScore.Score * weight;
                assessedWeight += weight;
            }

            return assessedWeight > 0f ? weightedTotal / assessedWeight : 0f;
        }
    }
}
