using UnityEngine;

namespace Medtriage.Simulation.Characters
{
    /// <summary>
    /// Drives the patient's Animator through the states described in Section 4.1 of
    /// Medtriage_Cardiac_Arrest_Scenario_Guide.docx. Build an Animator Controller with a
    /// default Unresponsive state and two Trigger parameters - "Rosc" and "Terminated" -
    /// transitioning to RoscBreathing / Terminated states.
    ///
    /// Part B of Medtriage_Cardiac_Arrest_Implementation_Walkthrough.docx.
    /// Animator is resolved defensively so the scene still runs before the controller
    /// asset exists, matching the guard style used in TaskManager and
    /// CardiacArrestSceneController.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class PatientController : MonoBehaviour
    {
        public const string RoscParameter = "Rosc";
        public const string TerminatedParameter = "Terminated";

        private static readonly int RoscTrigger = Animator.StringToHash(RoscParameter);
        private static readonly int TerminatedTrigger = Animator.StringToHash(TerminatedParameter);

        [SerializeField] private Animator animator;

        /// <summary>True once the patient has been driven to the ROSC state.</summary>
        public bool HasRosc { get; private set; }

        /// <summary>True once the patient has been driven to the terminated state.</summary>
        public bool IsTerminated { get; private set; }

        private void Awake()
        {
            if (animator == null) animator = GetComponent<Animator>();
        }

        public void SetRosc()
        {
            if (IsTerminated) return;
            HasRosc = true;
            if (animator != null && animator.runtimeAnimatorController != null)
                animator.SetTrigger(RoscTrigger);
        }

        public void SetTerminated()
        {
            if (HasRosc) return;
            IsTerminated = true;
            if (animator != null && animator.runtimeAnimatorController != null)
                animator.SetTrigger(TerminatedTrigger);
        }
    }
}
