using System;
using System.Collections;
using Medtriage.Shared.Data;
using Medtriage.Shared.Managers;
using UnityEngine;

namespace Medtriage.Simulation.Characters
{
    public enum TeammateState { Idle, Called, Performing, Done }

    /// <summary>
    /// A deliberately simple four-state AI teammate (Section 4.2 of
    /// Medtriage_Cardiac_Arrest_Scenario_Guide.docx) - reliability over emergent
    /// behavior. Call Assign() from the Team Delegation radial UI.
    ///
    /// Part B of Medtriage_Cardiac_Arrest_Implementation_Walkthrough.docx.
    /// Delegation is logged through TaskManager so team communication is auditable
    /// in the same event log the ScoringCalculator consumes.
    /// </summary>
    public class AITeammate : MonoBehaviour
    {
        [SerializeField] private string teammateName = "Nurse A";
        [SerializeField] private Animator animator;
        [SerializeField] private AudioSource voiceSource;
        [SerializeField] private AudioClip acknowledgeClip;
        [SerializeField, Min(0.1f)] private float defaultPerformSeconds = 4f;
        [SerializeField] private bool logToTaskManager = true;

        public string TeammateName => teammateName;
        public TeammateState State { get; private set; } = TeammateState.Idle;
        public TeammateTaskType? CurrentTask { get; private set; }

        public event Action<TeammateTaskType> OnTaskComplete;
        public event Action<TeammateState> OnStateChanged;

        public bool IsAvailable => State == TeammateState.Idle;

        public void Assign(TeammateTaskType task) => Assign(task, defaultPerformSeconds);

        public void Assign(TeammateTaskType task, float performSeconds)
        {
            if (State != TeammateState.Idle)
            {
                Debug.LogWarning($"[AITeammate] {teammateName} is already busy - ignoring new assignment.");
                return;
            }

            if (!gameObject.activeInHierarchy)
            {
                Debug.LogWarning($"[AITeammate] {teammateName} is inactive - cannot run a coroutine.");
                return;
            }

            CurrentTask = task;
            Log("teammate_task_assigned", task, true);
            StartCoroutine(RunTask(task, Mathf.Max(0.1f, performSeconds)));
        }

        private IEnumerator RunTask(TeammateTaskType task, float performSeconds)
        {
            SetState(TeammateState.Called);
            SetTrigger("Called");

            yield return new WaitForSeconds(1f); // brief "walking over" beat

            SetState(TeammateState.Performing);
            SetTrigger("Performing");
            if (voiceSource != null && acknowledgeClip != null) voiceSource.PlayOneShot(acknowledgeClip);

            yield return new WaitForSeconds(performSeconds);

            SetState(TeammateState.Done);
            Log("teammate_task_completed", task, true);
            OnTaskComplete?.Invoke(task);

            yield return new WaitForSeconds(0.5f);

            CurrentTask = null;
            SetState(TeammateState.Idle);
            SetTrigger("Idle");
        }

        private void OnDisable()
        {
            // Never leave the teammate stuck mid-coroutine if the scene unloads or the
            // object is pooled - the delegation UI must be able to reassign afterwards.
            if (State == TeammateState.Idle) return;
            StopAllCoroutines();
            CurrentTask = null;
            SetState(TeammateState.Idle);
        }

        private void SetState(TeammateState state)
        {
            State = state;
            OnStateChanged?.Invoke(state);
        }

        private void SetTrigger(string parameterName)
        {
            if (animator == null || animator.runtimeAnimatorController == null) return;
            animator.SetTrigger(parameterName);
        }

        private void Log(string eventName, TeammateTaskType task, bool correct)
        {
            if (!logToTaskManager) return;
            TaskManager manager = TaskManager.Instance;
            if (manager == null || !manager.IsRunning) return;
            manager.LogEvent(
                eventName,
                ScoreCategories.TeamCommunication,
                task.ToString(),
                $"{teammateName}:{task}",
                correct);
        }
    }
}
