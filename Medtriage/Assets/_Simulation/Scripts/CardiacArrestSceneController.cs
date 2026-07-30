using System;
using Medtriage.Shared.Data;
using Medtriage.Shared.Managers;
using Medtriage.Simulation.Networking;
using UnityEngine;

namespace Medtriage.Simulation
{
    /// <summary>Bridges a simulation scene to SessionManager, Cloud Save and the optional AI proxy.</summary>
    public class CardiacArrestSceneController : MonoBehaviour
    {
        [SerializeField] private ScenarioDefinition scenarioDefinition;
        [SerializeField] private TaskManager taskManager;
        [Header("Secure backend proxy URL - never an API key")]
        [SerializeField] private string debriefProxyUrl;
        [SerializeField, Min(1)] private int debriefTimeoutSeconds = 10;

        public TaskResult LastResult { get; private set; }
        public event Action<TaskResult> OnDebriefReady;

        private void Start()
        {
            if (scenarioDefinition == null || taskManager == null)
            {
                Debug.LogError("[CardiacArrestSceneController] ScenarioDefinition and TaskManager must be assigned.");
                enabled = false;
                return;
            }

            if (SessionManager.Instance == null)
                Debug.LogWarning("[CardiacArrestSceneController] No SessionManager. Start final tests from Bootstrap.");
            else if (!string.IsNullOrEmpty(SessionManager.Instance.CurrentTaskId) &&
                     SessionManager.Instance.CurrentTaskId != scenarioDefinition.ScenarioId)
                Debug.LogWarning("[CardiacArrestSceneController] TaskCatalog TaskId does not match ScenarioDefinition.ScenarioId.");

            taskManager.OnScenarioComplete += HandleScenarioComplete;
            taskManager.Begin(scenarioDefinition);
        }

        public void FinishScenario()
        {
            taskManager?.Complete();
        }

        private async void HandleScenarioComplete(TaskResult result)
        {
            if (result == null) return;
            LastResult = result;
            string aiSummary = null;

            try
            {
                aiSummary = await DebriefCoachingService.RequestSummaryAsync(
                    debriefProxyUrl, result, debriefTimeoutSeconds);
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"[CardiacArrestSceneController] AI debrief failed: {exception.Message}");
            }

            result.CoachingSummary = string.IsNullOrWhiteSpace(aiSummary)
                ? DebriefCoachingService.BuildLocalFallback(result)
                : aiSummary;

            if (SessionManager.Instance != null)
            {
                try { await SessionManager.Instance.RecordProgressAsync(result); }
                catch (Exception exception)
                {
                    Debug.LogError($"[CardiacArrestSceneController] Cloud Save failed: {exception.Message}");
                }
            }

            OnDebriefReady?.Invoke(result);
        }

        private void OnDestroy()
        {
            if (taskManager != null) taskManager.OnScenarioComplete -= HandleScenarioComplete;
        }
    }
}
