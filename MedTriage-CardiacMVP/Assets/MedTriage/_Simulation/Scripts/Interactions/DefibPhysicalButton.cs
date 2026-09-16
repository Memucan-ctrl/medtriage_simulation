using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System.Reflection;

namespace MedTriage.Simulation.Interactions
{
    public class DefibPhysicalButton : MonoBehaviour
    {
        public MonoBehaviour Defibrillator;
        public string MethodName = "BeginCharge";
        public Transform ButtonMesh;
        public float PressDepth = 0.006f;
        public float PressSeconds = 0.14f;
        public AudioClip ChargeWhineClip;
        public AudioClip ChargeReadyClip;
        public float ChargeSeconds = 3f;
        public float WhineVolume = 0.7f;
        public float ReadyVolume = 0.85f;

        public bool Charging { get; private set; }
        public bool ReadyFired { get; private set; }

        XRSimpleInteractable m_Interactable;
        AudioSource m_Audio;
        Vector3 m_RestLocal;
        bool m_HasRest;
        float m_Timer;

        void Awake()
        {
            m_Interactable = GetComponent<XRSimpleInteractable>();
            m_Audio = GetComponent<AudioSource>();
            if (m_Audio == null) m_Audio = gameObject.AddComponent<AudioSource>();
            m_Audio.playOnAwake = false;
            m_Audio.loop = false;
            m_Audio.spatialBlend = 1f;
            if (ButtonMesh != null) { m_RestLocal = ButtonMesh.localPosition; m_HasRest = true; }
        }

        void OnEnable() { if (m_Interactable != null) m_Interactable.selectEntered.AddListener(HandlePress); }
        void OnDisable() { if (m_Interactable != null) m_Interactable.selectEntered.RemoveListener(HandlePress); }

        void HandlePress(SelectEnterEventArgs args) { Press(); }

        public void Press()
        {
            if (Charging) return;
            Charging = true;
            ReadyFired = false;
            m_Timer = 0f;
            if (Defibrillator != null && !string.IsNullOrEmpty(MethodName))
            {
                var mi = Defibrillator.GetType().GetMethod(MethodName, BindingFlags.Public | BindingFlags.Instance);
                if (mi != null && mi.GetParameters().Length == 0) mi.Invoke(Defibrillator, null);
            }
            if (m_Audio != null && ChargeWhineClip != null)
            {
                m_Audio.Stop();
                m_Audio.clip = ChargeWhineClip;
                m_Audio.loop = false;
                m_Audio.volume = WhineVolume;
                m_Audio.Play();
            }
        }

        void Update()
        {
            if (Charging) m_Timer += Time.deltaTime;

            if (m_HasRest && ButtonMesh != null)
            {
                bool down = Charging && m_Timer < PressSeconds;
                ButtonMesh.localPosition = down ? m_RestLocal - new Vector3(0f, PressDepth, 0f) : m_RestLocal;
            }

            if (!Charging || ReadyFired) return;
            if (m_Timer < ChargeSeconds) return;

            ReadyFired = true;
            Charging = false;
            if (m_Audio != null)
            {
                m_Audio.Stop();
                if (ChargeReadyClip != null) m_Audio.PlayOneShot(ChargeReadyClip, ReadyVolume);
            }
        }
    }
}
