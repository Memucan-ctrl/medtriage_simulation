using UnityEngine;
using MedTriage.Shared.Data;
using MedTriage.Shared.Managers;

namespace MedTriage.Simulation.Interactions
{
    public enum DefibState
    {
        Idle,
        Charging,
        Charged,
        ClearCalled
    }

    public class DefibrillatorController : MonoBehaviour
    {
        public float ChargeSeconds = 3f;
        float chargeTimer = 0f;

        public DefibState State { get; private set; }
        public float ChargeProgress01 { get; private set; }
        public ShockDenyReason LastDenyReason { get; private set; }
        public int ShocksDelivered { get; private set; }

        public AudioClip ChargeWhineClip;
        public AudioClip ChargeReadyClip;
        public float WhineVolume = 0.7f;
        public float ReadyVolume = 0.85f;

        AudioSource m_Audio;

        void Awake()
        {
            m_Audio = GetComponent<AudioSource>();
            if (m_Audio == null) m_Audio = gameObject.AddComponent<AudioSource>();
            m_Audio.playOnAwake = false;
            m_Audio.loop = false;
            m_Audio.spatialBlend = 1f;

            if (ChargeWhineClip == null || ChargeReadyClip == null)
            {
                var btn = GetComponentInChildren<DefibPhysicalButton>(true);
                if (btn != null)
                {
                    if (ChargeWhineClip == null) ChargeWhineClip = btn.ChargeWhineClip;
                    if (ChargeReadyClip == null) ChargeReadyClip = btn.ChargeReadyClip;
                }
            }
        }

        public void AttachPads()
        {
            var tm = TaskManager.Instance;
            if (tm == null) return;
            tm.PadsAttached = true;
            tm.LogEvent(SimEventType.MonitorAttached, ScoreCategory.TechnicalExecution, 1f, 1f);
        }

        public void ClassifyRhythm(RhythmType chosen)
        {
            var tm = TaskManager.Instance;
            if (tm == null) return;
            bool correct = tm.Scenario != null && tm.Scenario.GroundTruthRhythm == chosen;
            tm.RhythmClassified = true;
            tm.ClassifiedRhythm = chosen;
            tm.LogEvent(SimEventType.RhythmClassified, ScoreCategory.DecisionMaking, correct ? 1f : 0f, 1f);
        }

        public void BeginCharge()
        {
            if (State == DefibState.Charging || State == DefibState.Charged) return;
            State = DefibState.Charging;
            chargeTimer = 0f;
            ChargeProgress01 = 0f;

            if (m_Audio != null && ChargeWhineClip != null)
            {
                m_Audio.Stop();
                m_Audio.clip = ChargeWhineClip;
                m_Audio.loop = false;
                m_Audio.volume = WhineVolume;
                m_Audio.Play();
            }
        }

        public void CallClear()
        {
            var tm = TaskManager.Instance;
            if (tm != null)
            {
                tm.ClearCalled = true;
                tm.LogEvent(SimEventType.ClosedLoopConfirmed, ScoreCategory.TeamCommunication, 1f, 1f);
            }
            if (State == DefibState.Charged) State = DefibState.ClearCalled;
        }

        void Update()
        {
            if (State != DefibState.Charging) return;

            chargeTimer += Time.deltaTime;
            ChargeProgress01 = ChargeSeconds > 0f ? Mathf.Clamp01(chargeTimer / ChargeSeconds) : 1f;

            if (ChargeProgress01 >= 1f)
            {
                State = DefibState.Charged;
                var tm = TaskManager.Instance;
                if (tm != null) tm.DefibCharged = true;

                if (m_Audio != null)
                {
                    m_Audio.Stop();
                    if (ChargeReadyClip != null) m_Audio.PlayOneShot(ChargeReadyClip, ReadyVolume);
                }
            }
        }

        public bool TryDeliverShock()
        {
            var tm = TaskManager.Instance;
            if (tm == null)
            {
                LastDenyReason = ShockDenyReason.NoActiveSession;
                return false;
            }

            ShockDenyReason reason;
            if (!tm.CanShock(out reason))
            {
                LastDenyReason = reason;
                if (reason == ShockDenyReason.ClearNotCalled) tm.TriggerCriticalError(CriticalErrorId.NoClearCall);
                return false;
            }

            bool shockable = tm.Scenario == null || tm.Scenario.GroundTruthRhythm == RhythmType.Shockable;

            LastDenyReason = ShockDenyReason.None;
            ShocksDelivered++;

            tm.LogEvent(SimEventType.ShockDelivered, ScoreCategory.ProtocolAdherence, shockable ? 1f : 0f, 1f);
            if (!shockable) tm.TriggerCriticalError(CriticalErrorId.ShockNonShockable);

            State = DefibState.Idle;
            ChargeProgress01 = 0f;
            chargeTimer = 0f;
            if (m_Audio != null && m_Audio.isPlaying && m_Audio.clip == ChargeWhineClip)
            {
                m_Audio.Stop();
            }
            tm.DefibCharged = false;
            tm.ClearCalled = false;
            return true;
        }
    }
}
