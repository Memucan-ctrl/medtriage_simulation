using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

namespace Medtriage.Simulation.Interactions
{
    /// <summary>
    /// Tracks a controller/hand transform against the patient's chest and derives
    /// compression rate, depth, and recoil (Section 3.3 and Section 6, Step 2.1 of
    /// Medtriage_Cardiac_Arrest_Scenario_Guide.docx). Includes the accessibility
    /// "compression assist" toggle from Section 3.3.
    ///
    /// The peak-detection here is a first-pass heuristic, not a finished signal
    /// processing pipeline - Section 10 of the scenario guide calls for a real
    /// calibration pass with several people of different heights/reach before you
    /// trust the numbers it produces.
    /// </summary>
    public class CompressionDetector : MonoBehaviour
    {
        [Header("Tracked point (controller or hand)")]
        [SerializeField] private Transform trackedPoint;
        [SerializeField] private Transform chestReferencePoint;

        [Header("Calibration - tune during the Section 10 calibration pass")]
        [SerializeField] private float targetDepthMeters = 0.05f;
        [SerializeField] private float depthToleranceMeters = 0.01f;
        [SerializeField] private int targetRateMin = 100;
        [SerializeField] private int targetRateMax = 120;
        [SerializeField] private float handsOffWarningSeconds = 10f;

        [Header("Accessibility (Section 3.3)")]
        [SerializeField] private bool compressionAssistEnabled = false;

        /// <summary>rate (per minute), depth (meters), goodRecoil</summary>
        public event Action<float, float, bool> OnCompressionSample;
        public event Action<float> OnHandsOffTooLong;

        public float TargetDepthMeters { get { return targetDepthMeters; } }
        public float DepthToleranceMeters { get { return depthToleranceMeters; } }
        public int TargetRateMin { get { return targetRateMin; } }
        public int TargetRateMax { get { return targetRateMax; } }
        public float HandsOffWarningSeconds { get { return handsOffWarningSeconds; } }
        public bool IsRunning { get { return isRunning; } }
        public bool CompressionAssistEnabled
        {
            get { return compressionAssistEnabled; }
            set { compressionAssistEnabled = value; }
        }

        private bool isRunning;
        private float lastPeakTime;
        private float peakDepth;
        private float lastCompressionTime;
        private bool goingDown;
        private bool warnedMissingRefs;
        private bool handsOffNotified;

        public void BeginTracking()
        {
            if (!compressionAssistEnabled && (trackedPoint == null || chestReferencePoint == null))
            {
                Debug.LogError("[CompressionDetector] BeginTracking called but trackedPoint / chestReferencePoint are not assigned. Assign them in the Inspector, or enable Compression Assist for desktop testing.", this);
                return;
            }

            isRunning = true;
            peakDepth = 0f;
            goingDown = false;
            handsOffNotified = false;
            lastCompressionTime = Time.time;
            lastPeakTime = Time.time;
        }

        public void StopTracking()
        {
            isRunning = false;
        }

        private void Update()
        {
            if (!isRunning) return;

            float currentDepth;
            if (compressionAssistEnabled)
            {
                currentDepth = SimulateAssistDepth();
            }
            else
            {
                if (trackedPoint == null || chestReferencePoint == null)
                {
                    if (!warnedMissingRefs)
                    {
                        warnedMissingRefs = true;
                        Debug.LogError("[CompressionDetector] trackedPoint / chestReferencePoint missing at runtime; tracking halted.", this);
                    }
                    isRunning = false;
                    return;
                }

                currentDepth = Vector3.Distance(trackedPoint.position, chestReferencePoint.position);
            }

            TrackPeaks(currentDepth);

            float handsOff = Time.time - lastCompressionTime;
            if (handsOff > handsOffWarningSeconds)
            {
                if (!handsOffNotified)
                {
                    handsOffNotified = true;
                    OnHandsOffTooLong(handsOff);
                }
            }
            else
            {
                handsOffNotified = false;
            }
        }

        private bool IsAssistHeld()
        {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            return Keyboard.current != null && Keyboard.current.spaceKey.isPressed;
#else
            return Input.GetKey(KeyCode.Space);
#endif
        }

        private float SimulateAssistDepth()
        {
            // Holding the assist key counts as a steady-rhythm compression without
            // requiring the full physical motion.
            return IsAssistHeld() ? targetDepthMeters : 0f;
        }

        private void TrackPeaks(float depth)
        {
            bool wasGoingDown = goingDown;
            goingDown = depth > peakDepth * 0.5f;
            peakDepth = Mathf.Max(peakDepth * 0.9f, depth); // decay so old peaks don't linger

            if (wasGoingDown && !goingDown)
            {
                float interval = Time.time - lastPeakTime;
                lastPeakTime = Time.time;
                lastCompressionTime = Time.time;
                if (interval <= 0f) return;

                float rate = 60f / interval;
                bool goodRecoil = depth < depthToleranceMeters;
                OnCompressionSample(rate, peakDepth, goodRecoil);
            }
        }

        public bool IsRateInRange(float rate)
        {
            return rate >= targetRateMin && rate <= targetRateMax;
        }

        public bool IsDepthInRange(float depth)
        {
            return depth >= targetDepthMeters - depthToleranceMeters;
        }
    }
}
