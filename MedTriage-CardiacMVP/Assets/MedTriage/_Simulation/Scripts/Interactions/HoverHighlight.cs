using System;
using System.Reflection;
using UnityEngine;

namespace MedTriage.Simulation.Interactions
{
    // Polls an XRI interactable's isHovered flag and tints the prop's renderers.
    // Polling avoids fragile reflection over generic UnityEvent listeners.
    [DisallowMultipleComponent]
    public class HoverHighlight : MonoBehaviour
    {
        public MonoBehaviour Interactable;
        public Color HighlightColor = new Color(0.30f, 0.85f, 1f);
        public float HighlightStrength = 0.55f;
        public AudioClip HoverClip;

        public bool IsHovered { get; private set; }

        Renderer[] m_Renderers;
        MaterialPropertyBlock m_Block;
        PropertyInfo m_HoverProp;
        AudioSource m_Audio;
        bool m_Applied;
        float m_Poll;

        void Start()
        {
            m_Block = new MaterialPropertyBlock();
            m_Renderers = GetComponentsInChildren<Renderer>(true);
            if (Interactable == null) Interactable = FindInteractable();
            if (Interactable != null)
                m_HoverProp = Interactable.GetType().GetProperty("isHovered", BindingFlags.Public | BindingFlags.Instance);
            if (HoverClip != null)
            {
                m_Audio = gameObject.AddComponent<AudioSource>();
                m_Audio.playOnAwake = false;
                m_Audio.spatialBlend = 0.8f;
            }
        }

        MonoBehaviour FindInteractable()
        {
            foreach (var mb in GetComponents<MonoBehaviour>())
            {
                if (mb == null || mb == this) continue;
                if (mb.GetType().GetProperty("isHovered", BindingFlags.Public | BindingFlags.Instance) != null) return mb;
            }
            return null;
        }

        void Update()
        {
            m_Poll -= Time.deltaTime;
            if (m_Poll > 0f) return;
            m_Poll = 0.08f;
            if (m_HoverProp == null || Interactable == null) return;
            object v = m_HoverProp.GetValue(Interactable, null);
            bool hov = v is bool && (bool)v;
            if (hov == m_Applied) return;
            m_Applied = hov;
            IsHovered = hov;
            Apply(hov);
            if (hov && m_Audio != null && HoverClip != null) m_Audio.PlayOneShot(HoverClip, 0.25f);
        }

        void Apply(bool on)
        {
            if (m_Renderers == null) return;
            Color emis = on ? HighlightColor * HighlightStrength : Color.black;
            foreach (var r in m_Renderers)
            {
                if (r == null) continue;
                r.GetPropertyBlock(m_Block);
                m_Block.SetColor("_EmissionColor", emis);
                m_Block.SetColor("_EmissiveColor", emis);
                r.SetPropertyBlock(m_Block);
            }
        }
    }
}