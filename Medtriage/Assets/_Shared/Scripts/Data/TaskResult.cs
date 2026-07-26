using System;
using System.Collections.Generic;
 
namespace Medtriage.Shared.Data
{
    /// <summary>
    /// The shared "result" contract both the Frontend and Simulation teams agree on
    /// (see Medtriage_Team_Development_Guide.docx, Section 8, and
    /// Medtriage_Cardiac_Arrest_Scenario_Guide.docx, Section 8). A Simulation scenario
    /// builds and populates one of these when it finishes; the Frontend saves it via
    /// CloudSaveManager and can show it on the MainMenu or a history screen.
    ///
    /// Kept deliberately simple (no Dictionary fields) so it serializes cleanly with
    /// UnityEngine.JsonUtility, which CloudSaveManager uses.
    /// </summary>
    [Serializable]
    public class TaskResult
    {
        public string TaskId;
        public long CompletedAtUnixSeconds;
 
        /// 0-100. Never includes critical errors in this average — see FlaggedCriticalErrors.
        public float CompositeScore;
 
        /// e.g. { "Efficiency", 82 }, { "Protocol Adherence", 91 }, ...
        public List<CategoryScore> CategoryScores = new List<CategoryScore>();
 
        /// Kept separate and non-averaged on purpose (Section 8.3 of the scenario guide).
        public List<string> FlaggedCriticalErrors = new List<string>();
 
        /// True whenever FlaggedCriticalErrors is non-empty; the debrief screen should
        /// visually gate on this rather than only showing the numeric score.
        public bool NeedsReview;
 
        /// Optional: the Gemini-generated coaching paragraph (Section 8.4 of the
        /// scenario guide). Left null/empty if the debrief call hasn't run yet.
        public string CoachingSummary;
    }
 
    [Serializable]
    public struct CategoryScore
    {
        public string Category;
        public float Score; // 0-100
    }
}