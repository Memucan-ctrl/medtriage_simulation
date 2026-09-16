using System.Collections.Generic;
using UnityEngine;

namespace MedTriage.Shared.Data
{
    public struct CategoryResult
    {
        public ScoreCategory Category;
        public float Raw01;
        public float NormalizedWeight;
        public bool HasData;
        public int EventCount;
    }

    public struct SessionResult
    {
        public float FinalScore;
        public float PenaltyPoints;
        public bool Failed;
        public List<CategoryResult> Categories;
        public List<CriticalErrorId> TriggeredErrors;
    }

    public static class ScoringCalculator
    {
        public static SessionResult Compute(ScenarioDefinition scenario, List<EventLogEntry> events, List<CriticalErrorId> triggeredErrors)
        {
            var result = new SessionResult();
            result.Categories = new List<CategoryResult>();
            result.TriggeredErrors = triggeredErrors != null ? new List<CriticalErrorId>(triggeredErrors) : new List<CriticalErrorId>();

            if (scenario == null)
            {
                result.FinalScore = 0f;
                result.Failed = true;
                return result;
            }

            var weightLookup = new Dictionary<ScoreCategory, float>();
            if (scenario.Weights != null)
            {
                for (int i = 0; i < scenario.Weights.Count; i++)
                {
                    var w = scenario.Weights[i];
                    if (w == null) continue;
                    weightLookup[w.Category] = Mathf.Max(0f, w.Weight);
                }
            }

            var categories = (ScoreCategory[])System.Enum.GetValues(typeof(ScoreCategory));
            float totalActiveWeight = 0f;

            for (int c = 0; c < categories.Length; c++)
            {
                var cat = categories[c];
                float weightedSum = 0f;
                float weightTotal = 0f;
                int count = 0;

                if (events != null)
                {
                    for (int e = 0; e < events.Count; e++)
                    {
                        var entry = events[e];
                        if (entry == null) continue;
                        if (entry.Category != cat) continue;
                        float w = Mathf.Max(0f, entry.Weight);
                        weightedSum += Mathf.Clamp01(entry.Score) * w;
                        weightTotal += w;
                        count++;
                    }
                }

                float declaredWeight = weightLookup.ContainsKey(cat) ? weightLookup[cat] : 0f;

                var cr = new CategoryResult();
                cr.Category = cat;
                cr.EventCount = count;
                cr.HasData = count > 0 && weightTotal > 0f;
                cr.Raw01 = cr.HasData ? weightedSum / weightTotal : 0f;
                cr.NormalizedWeight = cr.HasData ? declaredWeight : 0f;

                if (cr.HasData) totalActiveWeight += declaredWeight;

                result.Categories.Add(cr);
            }

            float score01 = 0f;
            for (int i = 0; i < result.Categories.Count; i++)
            {
                var cr = result.Categories[i];
                cr.NormalizedWeight = totalActiveWeight > 0f ? cr.NormalizedWeight / totalActiveWeight : 0f;
                result.Categories[i] = cr;
                score01 += cr.Raw01 * cr.NormalizedWeight;
            }

            float penalty = 0f;
            bool failed = false;
            if (scenario.CriticalErrors != null)
            {
                for (int i = 0; i < scenario.CriticalErrors.Count; i++)
                {
                    var rule = scenario.CriticalErrors[i];
                    if (rule == null) continue;
                    if (!result.TriggeredErrors.Contains(rule.Id)) continue;
                    penalty += Mathf.Max(0f, rule.PenaltyPoints);
                    if (rule.FailsSession) failed = true;
                }
            }

            result.PenaltyPoints = penalty;
            result.Failed = failed;
            result.FinalScore = Mathf.Clamp(score01 * 100f - penalty, 0f, 100f);
            if (failed) result.FinalScore = Mathf.Min(result.FinalScore, 49f);
            return result;
        }
    }
}
