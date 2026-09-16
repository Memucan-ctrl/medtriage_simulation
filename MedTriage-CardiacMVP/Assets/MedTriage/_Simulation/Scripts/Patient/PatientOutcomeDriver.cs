using UnityEngine;
using MedTriage.Simulation.Interactions;

namespace MedTriage.Simulation.Outcome
{
    public class PatientOutcomeDriver : MonoBehaviour
    {
        public DefibrillatorController Defibrillator;
        public Animator PatientAnimator;
        public int ShocksForRosc = 2;
        public float TerminateAfterSeconds = 300f;
        public bool AutoTerminate = true;

        private bool _rosc;
        private bool _terminated;
        private float _elapsed;

        void Update()
        {
            if (PatientAnimator == null) return;
            _elapsed += Time.deltaTime;

            if (!_rosc && !_terminated && Defibrillator != null && Defibrillator.ShocksDelivered >= ShocksForRosc)
            {
                ApplyRosc();
            }

            if (AutoTerminate && !_rosc && !_terminated && _elapsed >= TerminateAfterSeconds)
            {
                ApplyTerminate();
            }
        }

        public void ApplyRosc()
        {
            if (_rosc || PatientAnimator == null) return;
            _rosc = true;
            _terminated = false;
            PatientAnimator.SetBool("Terminated", false);
            PatientAnimator.SetBool("ROSC", true);
            PatientAnimator.gameObject.SendMessage("TriggerRecovery", SendMessageOptions.DontRequireReceiver);
        }

        public void ApplyTerminate()
        {
            if (_terminated || PatientAnimator == null) return;
            _terminated = true;
            PatientAnimator.SetBool("ROSC", false);
            PatientAnimator.SetBool("Terminated", true);
        }
    }
}