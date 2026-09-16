using UnityEngine;

namespace MedTriage.Simulation.Patient
{
    [DisallowMultipleComponent]
    public class PatientResponse : MonoBehaviour
    {
        public Animator PatientAnimator;
        public Transform ChestTarget;
        public Transform PrimaryHand;
        public Transform SecondaryHand;
        public float LateralRadiusMeters = 0.18f;
        public float MaxDeflectionMeters = 0.05f;
        public float SurfaceOffsetMeters = 0.01f;
        public float SmoothSpeed = 25f;
        public bool Metronome = true;
        public float MetronomeBpm = 110f;
        public bool MonitorBeep = true;
        public float MonitorBeepInterval = 4f;

        public float CurrentDeflectionMeters { get; private set; }

        Transform m_Sternum;
        AudioSource m_Audio;
        AudioClip m_Tick;
        AudioClip m_Beep;
        float m_NextTick;
        float m_NextBeep;
        float m_Smoothed;

        void Start()
        {
            if (PatientAnimator != null && PatientAnimator.isHuman)
            {
                m_Sternum = PatientAnimator.GetBoneTransform(HumanBodyBones.UpperChest);
                if (m_Sternum == null) m_Sternum = PatientAnimator.GetBoneTransform(HumanBodyBones.Chest);
            }
            m_Audio = gameObject.AddComponent<AudioSource>();
            m_Audio.playOnAwake = false;
            m_Audio.spatialBlend = 1f;
            m_Tick = MakeTone(880f, 0.05f, 0.35f);
            m_Beep = MakeTone(1400f, 0.12f, 0.30f);
        }

        AudioClip MakeTone(float freq, float seconds, float volume)
        {
            int rate = 44100;
            int count = Mathf.Max(16, Mathf.RoundToInt(rate * seconds));
            var data = new float[count];
            for (int i = 0; i < count; i++)
            {
                float t = (float)i / rate;
                float env = Mathf.Clamp01(1f - ((float)i / count));
                data[i] = Mathf.Sin(2f * Mathf.PI * freq * t) * volume * env;
            }
            var clip = AudioClip.Create("tone" + Mathf.RoundToInt(freq), count, 1, rate, false);
            clip.SetData(data, 0);
            return clip;
        }

        float Penetration(Transform hand)
        {
            if (hand == null || ChestTarget == null) return 0f;
            Vector3 d = hand.position - ChestTarget.position;
            Vector3 up = ChestTarget.up;
            float lateral = Vector3.ProjectOnPlane(d, up).magnitude;
            if (lateral > LateralRadiusMeters) return 0f;
            float vert = Vector3.Dot(d, up);
            if (vert > SurfaceOffsetMeters) return 0f;
            if (vert < -MaxDeflectionMeters - 0.25f) return 0f;
            return Mathf.Clamp(SurfaceOffsetMeters - vert, 0f, MaxDeflectionMeters);
        }

        void LateUpdate()
        {
            float target = Mathf.Max(Penetration(PrimaryHand), Penetration(SecondaryHand));
            m_Smoothed = Mathf.Lerp(m_Smoothed, target, Time.deltaTime * SmoothSpeed);
            CurrentDeflectionMeters = m_Smoothed;
            if (m_Sternum != null && m_Smoothed > 0.0005f && ChestTarget != null)
            {
                m_Sternum.position = m_Sternum.position - ChestTarget.up * m_Smoothed;
            }
            if (m_Audio == null) return;
            if (Metronome && MetronomeBpm > 1f && Time.time >= m_NextTick)
            {
                m_NextTick = Time.time + (60f / MetronomeBpm);
                m_Audio.PlayOneShot(m_Tick);
            }
            if (MonitorBeep && MonitorBeepInterval > 0.05f && Time.time >= m_NextBeep)
            {
                m_NextBeep = Time.time + MonitorBeepInterval;
                m_Audio.PlayOneShot(m_Beep);
            }
        }
    }
}