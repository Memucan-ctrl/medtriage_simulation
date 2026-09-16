using UnityEngine;
using System.Reflection;
using MedTriage.Simulation.Patient;
using MedTriage.Simulation.Vitals;
using MedTriage.Simulation.UI;

namespace MedTriage.Simulation.Outcome
{
    // Connects the pre-existing counters (shocks, compressions, epinephrine)
    // to the PatientPresence state machine. Without this nothing calls ApplyRosc.
    public class ArrestLoopBridge : MonoBehaviour
    {
        public PatientPresence Presence;
        public PatientVitalsMotion Vitals;
        public PatientMonitorDisplay Monitor;
        public MonoBehaviour Compression;
        public MonoBehaviour Defibrillator;
        public MonoBehaviour Syringe;

        public int ShocksForRosc = 2;
        public int CompressionsForRosc = 20;
        public bool RequireEpinephrine = true;
        public float SecondsInVfBeforeAsystole = 90f;
        public AudioClip FlatlineClip;
        public AudioSource FlatlineSource;
        public float FlatlineVolume = 0.30f;

        public int Shocks { get; private set; }
        public int Compressions { get; private set; }
        public bool EpiGiven { get; private set; }
        public bool Asystole { get; private set; }
        public bool RoscFired { get; private set; }

        AudioSource m_Audio;
        int m_LastShocks;
        float m_VfTimer;

        int m_LastCompressions;
        float m_LastActionTime;

        bool DefibBusy()
        {
            if (Defibrillator != null)
            {
                var p = Defibrillator.GetType().GetProperty("State");
                if (p != null)
                {
                    object v = null;
                    try { v = p.GetValue(Defibrillator, null); } catch { v = null; }
                    if (v != null)
                    {
                        string s = v.ToString();
                        if (s.Contains("Charg") || s.Contains("Clear")) return true;
                    }
                }
            }
            var pads = GetComponent<PadSocketArming>();
            if (pads != null && pads.BothSeated) return true;
            return false;
        }

        bool ActiveIntervention()
        {
            if (DefibBusy()) return true;
            if (Time.time - m_LastActionTime < 6f) return true;
            return false;
        }

        void Awake()
        {
            m_Audio = GetComponent<AudioSource>();
            if (m_Audio == null) m_Audio = gameObject.AddComponent<AudioSource>();
            m_Audio.playOnAwake = false;
            m_Audio.loop = false;
            m_Audio.spatialBlend = 1f;

            if (FlatlineSource == null)
            {
                var go = GameObject.Find("Audio_Flatline");
                if (go != null) FlatlineSource = go.GetComponent<AudioSource>();
            }
        }

        static object Read(MonoBehaviour c, string name)
        {
            if (c == null) return null;
            var t = c.GetType();
            var p = t.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
            if (p != null) return p.GetValue(c, null);
            var f = t.GetField(name, BindingFlags.Public | BindingFlags.Instance);
            if (f != null) return f.GetValue(c);
            return null;
        }

        void Update()
        {
            if (Presence == null) return;
            bool arrested = Presence.State.ToString() == "Arrested";

            object sd = Read(Defibrillator, "ShocksDelivered");
            if (sd != null) Shocks = (int)sd;
            object cc = Read(Compression, "CompressionCount");
            if (cc != null) Compressions = (int)cc;
            object ad = Read(Syringe, "Administered");
            if (ad != null) EpiGiven = (bool)ad;

            object cd = Read(Compression, "CurrentDepthMeters");
            float currentDepth = cd != null ? (float)cd : 0f;
            if (Vitals != null)
                Vitals.CompressionDepthMeters = arrested ? currentDepth : 0f;

            if (Compressions > m_LastCompressions || currentDepth > 0.01f)
            {
                m_LastCompressions = Compressions;
                m_LastActionTime = Time.time;
            }

            if (Shocks > m_LastShocks)
            {
                m_LastShocks = Shocks;
                m_VfTimer = 0f;
                m_LastActionTime = Time.time;
                if (Vitals != null) Vitals.Jolt();
            }

            if (arrested && !Asystole && !RoscFired)
            {
                if (!ActiveIntervention())
                {
                    m_VfTimer += Time.deltaTime;
                }
                else
                {
                    m_VfTimer = Mathf.Max(0f, m_VfTimer - Time.deltaTime * 0.5f);
                }

                if (m_VfTimer >= SecondsInVfBeforeAsystole)
                {
                    Asystole = true;
                    if (Monitor != null) Monitor.SetRhythm(PatientMonitorDisplay.Rhythm.Asystole);
                    if (FlatlineSource != null)
                    {
                        FlatlineSource.volume = FlatlineVolume;
                        if (!FlatlineSource.isPlaying) FlatlineSource.Play();
                    }
                    else if (m_Audio != null && FlatlineClip != null)
                    {
                        m_Audio.PlayOneShot(FlatlineClip, FlatlineVolume);
                    }
                }
            }

            if (!RoscFired && arrested)
            {
                bool epiOk = !RequireEpinephrine || EpiGiven;
                if (Shocks >= ShocksForRosc && epiOk && Compressions >= CompressionsForRosc)
                {
                    RoscFired = true;
                    if (FlatlineSource != null && FlatlineSource.isPlaying) FlatlineSource.Stop();
                    if (Vitals != null) Vitals.CompressionDepthMeters = 0f;
                    Presence.ApplyRosc();
                }
            }
        }
    }
}
