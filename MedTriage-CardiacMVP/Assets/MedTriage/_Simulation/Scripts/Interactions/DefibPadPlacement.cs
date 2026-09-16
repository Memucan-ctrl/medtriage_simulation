using System;
using System.Reflection;
using UnityEngine;

namespace MedTriage.Simulation.Interactions
{
    [DisallowMultipleComponent]
    public class DefibPadPlacement : MonoBehaviour
    {
        public MonoBehaviour AnteriorSocket;
        public MonoBehaviour LateralSocket;
        public MonoBehaviour Defibrillator;
        public AudioClip PadAttachClip;

        public bool AnteriorFilled { get; private set; }
        public bool LateralFilled { get; private set; }
        public bool PadsAttached { get; private set; }

        AudioSource m_Audio;
        float m_Poll;

        void Start()
        {
            m_Audio = gameObject.AddComponent<AudioSource>();
            m_Audio.playOnAwake = false;
            m_Audio.spatialBlend = 0.8f;
        }

        static bool SocketHasSelection(MonoBehaviour socket)
        {
            if (socket == null) return false;
            var t = socket.GetType();
            var p = t.GetProperty("hasSelection", BindingFlags.Public | BindingFlags.Instance);
            if (p != null)
            {
                object v = p.GetValue(socket, null);
                if (v is bool) return (bool)v;
            }
            var ip = t.GetProperty("interactablesSelected", BindingFlags.Public | BindingFlags.Instance);
            if (ip != null)
            {
                var list = ip.GetValue(socket, null) as System.Collections.ICollection;
                if (list != null) return list.Count > 0;
            }
            return false;
        }

        void Update()
        {
            m_Poll -= Time.deltaTime;
            if (m_Poll > 0f) return;
            m_Poll = 0.15f;

            AnteriorFilled = SocketHasSelection(AnteriorSocket);
            LateralFilled = SocketHasSelection(LateralSocket);

            if (PadsAttached || !AnteriorFilled || !LateralFilled) return;

            PadsAttached = true;
            if (m_Audio != null && PadAttachClip != null) m_Audio.PlayOneShot(PadAttachClip, 0.7f);
            if (Defibrillator != null)
            {
                var mi = Defibrillator.GetType().GetMethod("AttachPads", BindingFlags.Public | BindingFlags.Instance);
                if (mi != null)
                {
                    try { mi.Invoke(Defibrillator, null); Debug.Log("[MedTriage] Pads placed on both markers - AttachPads() called."); }
                    catch (Exception e) { Debug.LogWarning("[MedTriage] AttachPads failed: " + e.Message); }
                }
                else Debug.LogWarning("[MedTriage] AttachPads method not found on " + Defibrillator.GetType().Name);
            }
        }
    }
}