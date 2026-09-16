using UnityEngine;
using System.Reflection;

namespace MedTriage.Simulation.Interactions
{
    // Failure #6. Fires the shock when both pads are seated on the chest and the
    // unit is charged, so the operator never turns away to a GUI button.
    public class PadContactShock : MonoBehaviour
    {
        public MonoBehaviour Arming;
        public DefibrillatorController Defibrillator;
        public float SettleSeconds = 0.35f;
        public bool RequireCharged = true;
        public AudioClip ContactClip;
        public float ContactVolume = 0.5f;

        public bool Fired { get; private set; }
        public bool Seated { get; private set; }
        public string LastReason { get; private set; }

        AudioSource m_Audio;
        float m_SeatedFor;
        bool m_PlayedContact;

        void Awake()
        {
            m_Audio = GetComponent<AudioSource>();
            if (m_Audio == null) m_Audio = gameObject.AddComponent<AudioSource>();
            m_Audio.playOnAwake = false;
            m_Audio.loop = false;
            m_Audio.spatialBlend = 1f;
        }

        void Update()
        {
            if (Defibrillator == null || Arming == null) { LastReason = "missing refs"; return; }

            bool seated = false;
            var p = Arming.GetType().GetProperty("BothSeated", BindingFlags.Public | BindingFlags.Instance);
            if (p != null)
            {
                object v = p.GetValue(Arming, null);
                if (v is bool) seated = (bool)v;
            }
            Seated = seated;

            string state = Defibrillator.State.ToString();

            if (state == "Idle" || state == "Charging")
            {
                Fired = false;
                m_PlayedContact = false;
            }

            if (!seated)
            {
                m_SeatedFor = 0f;
                m_PlayedContact = false;
                LastReason = "pads not seated";
                return;
            }

            if (!m_PlayedContact)
            {
                m_PlayedContact = true;
                if (ContactClip != null) m_Audio.PlayOneShot(ContactClip, ContactVolume);
            }

            m_SeatedFor += Time.deltaTime;
            if (m_SeatedFor < SettleSeconds) { LastReason = "settling"; return; }
            if (Fired) { LastReason = "already fired"; return; }

            if (RequireCharged && state != "Charged" && state != "ClearCalled")
            {
                LastReason = "not charged: " + state;
                return;
            }

            Fired = true;
            Defibrillator.TryDeliverShock();
            LastReason = "fired at " + state;
        }
    }
}
