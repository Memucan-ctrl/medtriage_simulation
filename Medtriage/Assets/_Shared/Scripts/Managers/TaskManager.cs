using System;
using System.Collections.Generic;
using Medtriage.Shared.Data;
using UnityEngine;

namespace Medtriage.Shared.Managers
{
    public enum ScenarioPhase { Recognition, BasicLifeSupport, TeamRolesAndRhythm, ShockableBranch, NonShockableBranch, Cycles, Outcome }

    /// <summary>Reusable runtime that records auditable events and produces one TaskResult.</summary>
    public class TaskManager : MonoBehaviour
    {
        public static TaskManager Instance { get; private set; }
        public ScenarioDefinition Scenario { get; private set; }
        public ScenarioPhase CurrentPhase { get; private set; }
        public int CyclesElapsed { get; private set; }
        public bool IsShockableBranch { get; private set; }
        public bool IsRunning { get; private set; }
        public bool IsComplete { get; private set; }
        public IReadOnlyList<EventLogEntry> EventLog => eventLog;
        public IReadOnlyList<string> FlaggedCriticalErrors => flaggedCriticalErrorIds;

        public event Action<ScenarioPhase> OnPhaseChanged;
        public event Action<string> OnCriticalError;
        public event Action<TaskResult> OnScenarioComplete;

        private readonly List<EventLogEntry> eventLog = new List<EventLogEntry>();
        private readonly List<string> flaggedCriticalErrorIds = new List<string>();
        private float scenarioStartTime;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        public bool Begin(ScenarioDefinition scenario)
        {
            if (scenario == null || string.IsNullOrWhiteSpace(scenario.ScenarioId))
            {
                Debug.LogError("[TaskManager] A valid ScenarioDefinition is required.");
                return false;
            }

            Scenario = scenario;
            IsShockableBranch = scenario.GroundTruthRhythm == GroundTruthRhythm.Shockable;
            scenarioStartTime = Time.time;
            eventLog.Clear();
            flaggedCriticalErrorIds.Clear();
            CyclesElapsed = 0;
            IsComplete = false;
            IsRunning = true;
            SetPhase(ScenarioPhase.Recognition);
            return true;
        }

        public float ElapsedSeconds => IsRunning ? Mathf.Max(0f, Time.time - scenarioStartTime) : 0f;

        public void SetPhase(ScenarioPhase phase)
        {
            if (!IsRunning || IsComplete) return;
            CurrentPhase = phase;
            OnPhaseChanged?.Invoke(phase);
        }

        public void SetBranch(bool shockable)
        {
            if (!IsRunning || IsComplete) return;
            IsShockableBranch = shockable;
            SetPhase(shockable ? ScenarioPhase.ShockableBranch : ScenarioPhase.NonShockableBranch);
        }

        public void LogEvent(string eventName, string category, string expectedValue, string actualValue, bool correct)
        {
            if (!IsRunning || IsComplete) return;
            eventLog.Add(new EventLogEntry
            {
                EventName = eventName,
                Category = category,
                TimestampSeconds = ElapsedSeconds,
                ExpectedValue = expectedValue,
                ActualValue = actualValue,
                Correct = correct
            });
        }

        public void FlagCriticalError(string ruleId)
        {
            if (!IsRunning || IsComplete || string.IsNullOrWhiteSpace(ruleId) || flaggedCriticalErrorIds.Contains(ruleId)) return;
            flaggedCriticalErrorIds.Add(ruleId);
            OnCriticalError?.Invoke(ruleId);
        }

        public void CompleteCycle()
        {
            if (!IsRunning || IsComplete) return;
            CyclesElapsed++;
            LogEvent("cycle_completed", string.Empty, string.Empty, CyclesElapsed.ToString(), true);
        }

        public TaskResult Complete()
        {
            if (!IsRunning || IsComplete || Scenario == null) return null;
            IsComplete = true;
            IsRunning = false;
            CurrentPhase = ScenarioPhase.Outcome;
            OnPhaseChanged?.Invoke(CurrentPhase);

            List<CategoryScore> scores = ScoringCalculator.ComputeCategoryScores(eventLog);
            var result = new TaskResult
            {
                TaskId = Scenario.ScenarioId,
                CompletedAtUnixSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                CategoryScores = scores,
                FlaggedCriticalErrors = new List<string>(flaggedCriticalErrorIds),
                NeedsReview = flaggedCriticalErrorIds.Count > 0
            };
            result.CompositeScore = ScoringCalculator.ComputeComposite(scores, Scenario.Weighting);
            OnScenarioComplete?.Invoke(result);
            return result;
        }
    }
}
