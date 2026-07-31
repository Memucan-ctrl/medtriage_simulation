using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Medtriage.Shared.Data;
using Medtriage.Shared.Managers;

namespace Medtriage.Simulation.UI
{
    /// <summary>
    /// The 2-minute cycle ring and the "Rotate compressor?" prompt from Section 6,
    /// Steps 6.1-6.2 of Medtriage_Cardiac_Arrest_Scenario_Guide.docx.
    /// </summary>
    public class CycleTimerHUD : MonoBehaviour
    {
        [SerializeField] private Image ringFill;
        [SerializeField] private TMP_Text timeLabel;
        [SerializeField] private GameObject rotatePrompt;

        private float cycleDuration;
        private float timeRemaining;
        private bool running;
        private int consecutiveCyclesWithoutRotation;

        public bool IsRunning { get { return running; } }
        public float TimeRemaining { get { return timeRemaining; } }

        public void StartCycle(float durationSeconds)
        {
            if (durationSeconds <= 0f)
            {
                Debug.LogError("[CycleTimerHUD] StartCycle called with a non-positive duration; ignoring.", this);
                return;
            }

            cycleDuration = durationSeconds;
            timeRemaining = durationSeconds;
            running = true;
            if (rotatePrompt != null) rotatePrompt.SetActive(false);
        }

        public void StopCycle()
        {
            running = false;
        }

        private void Update()
        {
            if (!running) return;

            timeRemaining -= Time.deltaTime;

            if (ringFill != null) ringFill.fillAmount = Mathf.Clamp01(timeRemaining / cycleDuration);
            if (timeLabel != null) timeLabel.text = TimeSpan.FromSeconds(Mathf.Max(0f, timeRemaining)).ToString(@"m\:ss");

            if (timeRemaining <= 0f)
            {
                running = false;

                if (TaskManager.Instance != null)
                    TaskManager.Instance.CompleteCycle();
                else
                    Debug.LogWarning("[CycleTimerHUD] Cycle ended but TaskManager.Instance is null; the cycle was not recorded.", this);

                if (rotatePrompt != null) rotatePrompt.SetActive(true);
            }
        }

        /// <summary>Wire both the Yes and No buttons on the rotate prompt to this, with the matching bool.</summary>
        public void AnswerRotatePrompt(bool rotated)
        {
            consecutiveCyclesWithoutRotation = rotated ? 0 : consecutiveCyclesWithoutRotation + 1;

            if (TaskManager.Instance != null)
            {
                TaskManager.Instance.LogEvent(
                    "compressor_rotation",
                    ScoreCategories.TechnicalExecution,
                    expectedValue: "rotated=true",
                    actualValue: string.Format("rotated={0}", rotated),
                    correct: rotated || consecutiveCyclesWithoutRotation < 2);
            }
            else
            {
                Debug.LogWarning("[CycleTimerHUD] Rotate prompt answered but TaskManager.Instance is null; not logged.", this);
            }

            if (rotatePrompt != null) rotatePrompt.SetActive(false);

            if (cycleDuration > 0f) StartCycle(cycleDuration);
        }
    }
}
