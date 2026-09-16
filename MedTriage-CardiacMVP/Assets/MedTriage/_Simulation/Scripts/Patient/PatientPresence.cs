using UnityEngine;
using MedTriage.Simulation.UI;
using MedTriage.Simulation.Vitals;

namespace MedTriage.Simulation.Patient
{
    public enum PresenceState { Awake, Monitored, Deteriorating, Arrested, Rosc }

    public class PatientPresence : MonoBehaviour
    {
        public Transform HeadBone;
        public PatientVitalsMotion Vitals;
        public PatientMonitorDisplay Monitor;
        public Vector3 FaceDirectionAtRest = Vector3.up;
        public float MaxHeadTurnDegrees = 42f;
        public float HeadTurnSpeed = 2.2f;
        public float SecondsBeforeDeterioration = 18f;
        public float DeteriorationSeconds = 40f;
        public float StartHeartRate = 82f;
        public float PeakHeartRate = 150f;
        public float StartSpO2 = 97f;
        public float EndSpO2 = 78f;
        public float CalmBreathsPerMinute = 14f;
        public float ShallowBreathsPerMinute = 30f;
        public float RoscHeartRate = 96f;

        public PresenceState State { get; private set; }

        Transform m_Cam;
        Quaternion m_RestLocal;
        bool m_Cached;
        float m_StateTime;

        void Start()
        {
            if (Vitals == null) Vitals = GetComponentInChildren<PatientVitalsMotion>(true);
            Enter(PresenceState.Awake);
        }

        public void OnOximeterAttached()
        {
            if (State != PresenceState.Awake) return;
            Enter(PresenceState.Monitored);
        }

        public void ForceArrest() { Enter(PresenceState.Arrested); }
        public void ApplyRosc() { Enter(PresenceState.Rosc); }
        public void Jolt() { if (Vitals != null) Vitals.Jolt(); }

        void Enter(PresenceState s)
        {
            State = s;
            m_StateTime = 0f;
            if (s == PresenceState.Awake)
            {
                if (Monitor != null) { Monitor.PowerOn = false; Monitor.SetRhythm(PatientMonitorDisplay.Rhythm.SinusRhythm); Monitor.HeartRate = StartHeartRate; Monitor.SpO2 = StartSpO2; }
                if (Vitals != null) { Vitals.Breathing = true; Vitals.BreathsPerMinute = CalmBreathsPerMinute; }
            }
            else if (s == PresenceState.Monitored)
            {
                if (Monitor != null) { Monitor.PowerOn = true; Monitor.SetRhythm(PatientMonitorDisplay.Rhythm.SinusRhythm); Monitor.HeartRate = StartHeartRate; Monitor.SpO2 = StartSpO2; }
                if (Vitals != null) { Vitals.Breathing = true; Vitals.BreathsPerMinute = CalmBreathsPerMinute; }
            }
            else if (s == PresenceState.Arrested)
            {
                if (Monitor != null) { Monitor.PowerOn = true; Monitor.SetRhythm(PatientMonitorDisplay.Rhythm.VentricularFibrillation); }
                if (Vitals != null) { Vitals.Jolt(); Vitals.StopBreathing(); Vitals.Breathing = false; }
            }
            else if (s == PresenceState.Rosc)
            {
                if (Monitor != null) { Monitor.PowerOn = true; Monitor.SetRhythm(PatientMonitorDisplay.Rhythm.SinusRhythm); Monitor.HeartRate = RoscHeartRate; Monitor.SpO2 = 92f; }
                if (Vitals != null) { Vitals.Breathing = true; Vitals.BreathsPerMinute = 12f; }
            }
        }

        void Update()
        {
            m_StateTime += Time.deltaTime;
            if (State == PresenceState.Monitored && m_StateTime >= SecondsBeforeDeterioration) Enter(PresenceState.Deteriorating);
            if (State != PresenceState.Deteriorating) return;
            float k = Mathf.Clamp01(m_StateTime / Mathf.Max(1f, DeteriorationSeconds));
            if (Monitor != null) { Monitor.HeartRate = Mathf.Lerp(StartHeartRate, PeakHeartRate, k); Monitor.SpO2 = Mathf.Lerp(StartSpO2, EndSpO2, k); }
            if (Vitals != null) Vitals.BreathsPerMinute = Mathf.Lerp(CalmBreathsPerMinute, ShallowBreathsPerMinute, k);
            if (k >= 1f) Enter(PresenceState.Arrested);
        }

        void LateUpdate()
        {
            if (HeadBone == null) return;
            if (!m_Cached) { m_RestLocal = HeadBone.localRotation; m_Cached = true; }
            if (m_Cam == null && Camera.main != null) m_Cam = Camera.main.transform;
            Quaternion restWorld = HeadBone.parent != null ? HeadBone.parent.rotation * m_RestLocal : m_RestLocal;
            Quaternion target = restWorld;
            bool aware = State != PresenceState.Arrested;
            if (aware && m_Cam != null)
            {
                Vector3 want = (m_Cam.position - HeadBone.position).normalized;
                if (want.sqrMagnitude > 0.0001f)
                {
                    Quaternion delta = Quaternion.FromToRotation(FaceDirectionAtRest.normalized, want);
                    delta = Quaternion.RotateTowards(Quaternion.identity, delta, MaxHeadTurnDegrees);
                    target = delta * restWorld;
                }
            }
            HeadBone.rotation = Quaternion.Slerp(HeadBone.rotation, target, Time.deltaTime * HeadTurnSpeed);
        }
    }
}
