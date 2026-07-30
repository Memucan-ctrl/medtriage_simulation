using System;
using System.Collections;
using Medtriage.Shared.Data;
using Medtriage.Shared.Managers;
using UnityEngine;

namespace Medtriage.Simulation.Interactions
{
    public enum DefibrillatorState { Idle, Charging, Charged, ClearCalled }

    /// <summary>Simulation-safe charge, clear and shock sequence with auditable errors.</summary>
    public class DefibrillatorController : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float chargeSeconds = 3f;
        public DefibrillatorState State { get; private set; } = DefibrillatorState.Idle;
        public event Action<DefibrillatorState> OnStateChanged;
        public event Action OnShockDelivered;

        private bool clearCalled;

        public void BeginCharge()
        {
            if (State != DefibrillatorState.Idle) return;
            StartCoroutine(ChargeRoutine());
        }

        private IEnumerator ChargeRoutine()
        {
            SetState(DefibrillatorState.Charging);
            yield return new WaitForSeconds(chargeSeconds);
            SetState(DefibrillatorState.Charged);
        }

        public void CallClear()
        {
            if (State != DefibrillatorState.Charged) return;
            clearCalled = true;
            SetState(DefibrillatorState.ClearCalled);
        }

        public void Shock()
        {
            if (State != DefibrillatorState.Charged && State != DefibrillatorState.ClearCalled) return;
            TaskManager manager = TaskManager.Instance;
            if (manager == null) return;

            bool shockable = manager.Scenario != null &&
                             manager.Scenario.GroundTruthRhythm == GroundTruthRhythm.Shockable;
            if (!clearCalled) manager.FlagCriticalError("no_clear_call");
            if (!shockable) manager.FlagCriticalError("shock_non_shockable");

            manager.LogEvent(
                "shock_delivered",
                ScoreCategories.ProtocolAdherence,
                "shockable=true;clear_called=true",
                $"shockable={shockable};clear_called={clearCalled}",
                shockable && clearCalled);

            OnShockDelivered?.Invoke();
            clearCalled = false;
            SetState(DefibrillatorState.Idle);
        }

        private void SetState(DefibrillatorState state)
        {
            State = state;
            OnStateChanged?.Invoke(state);
        }
    }
}
