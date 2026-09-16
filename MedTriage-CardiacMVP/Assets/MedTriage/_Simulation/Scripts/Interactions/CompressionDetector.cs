using UnityEngine;
using MedTriage.Shared.Data;
using MedTriage.Shared.Managers;

namespace MedTriage.Simulation.Interactions
{
    public class CompressionDetector : MonoBehaviour
    {
        public Transform ChestTarget;
        public Transform CompressorHand;

        public float TargetDepthMeters = 0.05f;
        public float DepthToleranceMeters = 0.01f;
        public float MinRatePerMinute = 100f;
        public float MaxRatePerMinute = 120f;
        public float HandsOffWarningSeconds = 10f;
        public float EngageRadiusMeters = 0.15f;
        public float ReleaseDepthMeters = 0.01f;

        public float CurrentDepthMeters { get; private set; }
        public float CurrentRatePerMinute { get; private set; }
        public bool HandsOnChest { get; private set; }
        public int CompressionCount { get; private set; }
        public float LastCompressionDepthMeters { get; private set; }

        bool inStroke;
        float strokeMaxDepth;
        float lastCompressionRealtime = -1f;
        bool handsOffLogged;

        void Update()
        {
            if (ChestTarget == null || CompressorHand == null) return;

            var tm = TaskManager.Instance;

            Vector3 toHand = CompressorHand.position - ChestTarget.position;
            float lateral = Vector3.ProjectOnPlane(toHand, ChestTarget.up).magnitude;
            HandsOnChest = lateral <= EngageRadiusMeters;

            float depth = Vector3.Dot(ChestTarget.position - CompressorHand.position, ChestTarget.up);
            CurrentDepthMeters = HandsOnChest ? Mathf.Max(0f, depth) : 0f;

            if (!HandsOnChest)
            {
                inStroke = false;
                strokeMaxDepth = 0f;
            }
            else if (!inStroke)
            {
                if (CurrentDepthMeters > ReleaseDepthMeters)
                {
                    inStroke = true;
                    strokeMaxDepth = CurrentDepthMeters;
                }
            }
            else
            {
                if (CurrentDepthMeters > strokeMaxDepth) strokeMaxDepth = CurrentDepthMeters;
                if (CurrentDepthMeters <= ReleaseDepthMeters)
                {
                    RegisterCompression(strokeMaxDepth, tm);
                    inStroke = false;
                    strokeMaxDepth = 0f;
                }
            }

            if (tm != null && tm.SessionActive)
            {
                float since = tm.SecondsSinceLastCompression();
                if (since >= HandsOffWarningSeconds)
                {
                    if (!handsOffLogged)
                    {
                        handsOffLogged = true;
                        tm.LogEvent(SimEventType.CompressionInterruption, ScoreCategory.Efficiency, 0f, 1f);
                    }
                }
                else
                {
                    handsOffLogged = false;
                }

                if (since >= 30f) tm.TriggerCriticalError(CriticalErrorId.NoCompressions30s);
            }
        }

        void RegisterCompression(float peakDepth, TaskManager tm)
        {
            CompressionCount++;
            LastCompressionDepthMeters = peakDepth;

            float now = Time.time;
            if (lastCompressionRealtime > 0f)
            {
                float interval = now - lastCompressionRealtime;
                if (interval > 0.05f) CurrentRatePerMinute = 60f / interval;
            }
            lastCompressionRealtime = now;

            if (tm == null) return;
            tm.NotifyCompression();

            tm.LogEvent(SimEventType.CompressionStream, ScoreCategory.TechnicalExecution, StrokeQuality01(peakDepth), 1f);
        }

        public float DepthQuality01(float peakDepth)
        {
            float error = Mathf.Abs(peakDepth - TargetDepthMeters);
            float allowed = Mathf.Max(0f, error - DepthToleranceMeters);
            if (TargetDepthMeters <= 0f) return 0f;
            return Mathf.Clamp01(1f - allowed / TargetDepthMeters);
        }

        public float RateQuality01()
        {
            if (CurrentRatePerMinute <= 0f) return 1f;
            if (CurrentRatePerMinute < MinRatePerMinute) return Mathf.Clamp01(CurrentRatePerMinute / MinRatePerMinute);
            if (CurrentRatePerMinute > MaxRatePerMinute) return Mathf.Clamp01(MaxRatePerMinute / CurrentRatePerMinute);
            return 1f;
        }

        public float StrokeQuality01(float peakDepth)
        {
            return (DepthQuality01(peakDepth) + RateQuality01()) * 0.5f;
        }
    }
}
