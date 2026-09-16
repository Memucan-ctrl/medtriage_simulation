using UnityEngine;

namespace MedTriage.Simulation.Vitals
{
    // Hand-driven motion for a statically posed patient.
    // The Animator is disabled because the only clips in the project are
    // standing Mixamo clips, which cannot play on a supine body.
    public class PatientVitalsMotion : MonoBehaviour
    {
        public Transform ChestBone;
        public bool Breathing = true;
        public float BreathsPerMinute = 10f;
        public float RiseMeters = 0.010f;
        public float JoltMeters = 0.035f;
        public float JoltSeconds = 0.28f;

        // Driven each frame by ArrestLoopBridge from CompressionDetector.
        public float CompressionDepthMeters = 0f;
        public float CompressionScale = 1f;

        Vector3 m_Base;
        bool m_Ready;
        float m_JoltEnd = -1f;

        void Start()
        {
            if (ChestBone == null) ChestBone = FindChest(transform);
            if (ChestBone == null) { enabled = false; return; }
            m_Base = ChestBone.position;
            m_Ready = true;
        }

        static Transform FindChest(Transform root)
        {
            Transform best = null;
            foreach (var t in root.GetComponentsInChildren<Transform>(true))
            {
                string n = t.name.ToLowerInvariant();
                if (n.Contains("twist") || n.Contains("scalecompensation")) continue;
                if (n.Contains("spine02") || n.Contains("spine2")) return t;
                if (best == null && n.Contains("spine01")) best = t;
                if (best == null && n.Contains("chest")) best = t;
            }
            return best;
        }

        // Call on shock delivery for a visible twitch.
        public void Jolt() { m_JoltEnd = Time.time + JoltSeconds; }

        // Call when the patient is declared dead.
        public void StopBreathing() { Breathing = false; }

        void LateUpdate()
        {
            if (!m_Ready) return;
            float y = 0f;
            if (Breathing && BreathsPerMinute > 0.01f)
            {
                float hz = BreathsPerMinute / 60f;
                float s = Mathf.Sin(Time.time * hz * 2f * Mathf.PI);
                y += s * 0.5f * RiseMeters + 0.5f * RiseMeters;
            }
            if (Time.time < m_JoltEnd)
            {
                float k = (m_JoltEnd - Time.time) / Mathf.Max(0.01f, JoltSeconds);
                y += Mathf.Sin(k * 18f) * JoltMeters * k;
            }
            if (CompressionDepthMeters > 0f) y -= CompressionDepthMeters * CompressionScale;
            ChestBone.position = m_Base + new Vector3(0f, y, 0f);
        }
    }
}
