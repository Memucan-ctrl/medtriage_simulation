using System;
using System.Collections.Generic;
using UnityEngine;
using MedTriage.Shared.Data;

namespace MedTriage.Shared.Managers
{
    public enum ShockDenyReason
    {
        None,
        NoActiveSession,
        PadsNotAttached,
        RhythmNotClassified,
        RhythmNotShockable,
        DefibNotCharged,
        ClearNotCalled
    }

    public class TaskManager : MonoBehaviour
    {
        static TaskManager instance;
        public static TaskManager Instance { get { return instance; } }
        public static bool Exists { get { return instance != null; } }

        public ScenarioDefinition Scenario;

        readonly List<EventLogEntry> events = new List<EventLogEntry>();
        readonly List<CriticalErrorId> triggeredErrors = new List<CriticalErrorId>();

        public ScenarioPhase Phase { get; private set; }
        public float SessionTime { get; private set; }
        public bool SessionActive { get; private set; }
        public int CompletedCycles { get; private set; }

        public bool PadsAttached;
        public bool RhythmClassified;
        public RhythmType ClassifiedRhythm;
        public bool ClearCalled;
        public bool DefibCharged;
        public float LastCompressionTime { get; private set; }

        public event Action<EventLogEntry> EventLogged;
        public event Action<SessionResult> SessionFinished;

        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this);
                return;
            }
            instance = this;
        }

        void OnDestroy()
        {
            if (instance == this) instance = null;
        }

        void Update()
        {
            if (SessionActive) SessionTime += Time.deltaTime;
        }

        public void BeginSession()
        {
            events.Clear();
            triggeredErrors.Clear();
            SessionTime = 0f;
            CompletedCycles = 0;
            Phase = ScenarioPhase.Recognition;
            PadsAttached = false;
            RhythmClassified = false;
            ClearCalled = false;
            DefibCharged = false;
            LastCompressionTime = -1f;
            SessionActive = true;
        }

        public void SetPhase(ScenarioPhase phase)
        {
            Phase = phase;
        }

        public void NotifyCompression()
        {
            LastCompressionTime = SessionTime;
        }

        public float SecondsSinceLastCompression()
        {
            if (LastCompressionTime < 0f) return SessionTime;
            return SessionTime - LastCompressionTime;
        }

        public void CompleteCycle()
        {
            CompletedCycles++;
            LogEvent(SimEventType.CycleCompleted, ScoreCategory.Efficiency, 1f, 1f);
        }

        public EventLogEntry LogEvent(SimEventType type, ScoreCategory category, float score, float weight)
        {
            var entry = new EventLogEntry();
            entry.EventType = type;
            entry.Category = category;
            entry.Score = Mathf.Clamp01(score);
            entry.Weight = Mathf.Max(0f, weight);
            entry.TimestampSeconds = SessionTime;
            events.Add(entry);

            var handler = EventLogged;
            if (handler != null) handler(entry);
            return entry;
        }

        public void TriggerCriticalError(CriticalErrorId id)
        {
            if (!triggeredErrors.Contains(id)) triggeredErrors.Add(id);
        }

        public bool HasEvent(SimEventType type)
        {
            for (int i = 0; i < events.Count; i++)
            {
                if (events[i].EventType == type) return true;
            }
            return false;
        }

        public int CountEvents(SimEventType type)
        {
            int n = 0;
            for (int i = 0; i < events.Count; i++)
            {
                if (events[i].EventType == type) n++;
            }
            return n;
        }

        public bool CanShock(out ShockDenyReason reason)
        {
            reason = ShockDenyReason.None;
            if (!SessionActive) { reason = ShockDenyReason.NoActiveSession; return false; }
            if (!PadsAttached) { reason = ShockDenyReason.PadsNotAttached; return false; }
            if (!RhythmClassified) { reason = ShockDenyReason.RhythmNotClassified; return false; }
            if (!DefibCharged) { reason = ShockDenyReason.DefibNotCharged; return false; }
            if (!ClearCalled) { reason = ShockDenyReason.ClearNotCalled; return false; }
            return true;
        }

        public SessionResult Finish()
        {
            SessionActive = false;
            var result = ScoringCalculator.Compute(Scenario, events, triggeredErrors);
            var handler = SessionFinished;
            if (handler != null) handler(result);
            return result;
        }

        public List<EventLogEntry> GetEventsCopy()
        {
            return new List<EventLogEntry>(events);
        }
    }
}
