using System;
using System.Reflection;
using UnityEngine;
using MedTriage.Simulation.UI;

namespace MedTriage.Simulation.Managers
{
    [DisallowMultipleComponent]
    public class ScenarioDirector : MonoBehaviour
    {
        public PatientMonitorDisplay Monitor;
        public MedicationTray Tray;
        public MonoBehaviour Defibrillator;
        public MonoBehaviour Compression;
        public Animator PatientAnimator;
        public AudioClip CodeCallClip;
        public AudioClip BreathingClip;

        [Tooltip("Shocks required before ROSC is possible.")]
        public int ShocksRequiredForRosc = 2;
        [Tooltip("Require correct epinephrine before ROSC.")]
        public bool RequireEpinephrine = true;
        public int CompressionsRequiredForRosc = 20;

        public bool RoscAchieved { get; private set; }
        public float SessionStartTime { get; private set; }
        public float TimeToFirstCompression { get; private set; }
        public float TimeToFirstShock { get; private set; }

        AudioSource m_Audio;
        int m_LastShocks;
        bool m_Started;

        void Start()
        {
            m_Audio = gameObject.AddComponent<AudioSource>();
            m_Audio.playOnAwake = false;
            m_Audio.spatialBlend = 1f;
            SessionStartTime = Time.time;
            TimeToFirstCompression = -1f;
            TimeToFirstShock = -1f;
            if (Monitor != null) Monitor.SetRhythm(PatientMonitorDisplay.Rhythm.VentricularFibrillation);
            if (CodeCallClip != null) m_Audio.PlayOneShot(CodeCallClip, 0.6f);
            m_Started = true;
        }

        static object Read(object o, string n)
        {
            if (o == null) return null;
            var t = o.GetType();
            var p = t.GetProperty(n, BindingFlags.Public | BindingFlags.Instance);
            if (p != null) return p.GetValue(o, null);
            var f = t.GetField(n, BindingFlags.Public | BindingFlags.Instance);
            if (f != null) return f.GetValue(o);
            return null;
        }

        static int ReadInt(object o, string n) { var v = Read(o, n); return v is int ? (int)v : 0; }

        void Update()
        {
            if (!m_Started) return;

            int comps = ReadInt(Compression, "CompressionCount");
            if (comps > 0 && TimeToFirstCompression < 0f) TimeToFirstCompression = Time.time - SessionStartTime;

            int shocks = ReadInt(Defibrillator, "ShocksDelivered");
            if (shocks > 0 && TimeToFirstShock < 0f) TimeToFirstShock = Time.time - SessionStartTime;

            if (shocks > m_LastShocks)
            {
                m_LastShocks = shocks;
                // Refractory VF: the rhythm stays shockable across cycles until ROSC conditions are met.
            }

            if (RoscAchieved) return;

            bool epiOk = !RequireEpinephrine || (Tray != null && Tray.EpinephrineGiven);
            bool shockOk = shocks >= ShocksRequiredForRosc;
            bool cprOk = comps >= CompressionsRequiredForRosc;
            if (epiOk && shockOk && cprOk) AchieveRosc();
        }

        public void AchieveRosc()
        {
            if (RoscAchieved) return;
            RoscAchieved = true;
            if (Monitor != null) Monitor.SetRhythm(PatientMonitorDisplay.Rhythm.SinusRhythm);
            if (PatientAnimator != null)
            {
                foreach (var p in PatientAnimator.parameters)
                {
                    if (p.type == AnimatorControllerParameterType.Bool && p.name == "ROSC")
                        PatientAnimator.SetBool("ROSC", true);
                }
            }
            if (PatientAnimator != null) PatientAnimator.gameObject.SendMessage("TriggerRecovery", SendMessageOptions.DontRequireReceiver);
            else gameObject.SendMessage("TriggerRecovery", SendMessageOptions.DontRequireReceiver);
            if (m_Audio != null && BreathingClip != null) m_Audio.PlayOneShot(BreathingClip, 0.85f);
            Debug.Log("[MedTriage] ROSC achieved.");
        }

        public string BuildDebrief()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("=== DEBRIEF ===");
            sb.AppendLine("Outcome: " + (RoscAchieved ? "ROSC achieved" : "No ROSC"));
            sb.AppendLine("Time to first compression: " + (TimeToFirstCompression >= 0f ? TimeToFirstCompression.ToString("F1") + " s" : "never"));
            sb.AppendLine("Time to first shock: " + (TimeToFirstShock >= 0f ? TimeToFirstShock.ToString("F1") + " s" : "never"));
            sb.AppendLine("Compressions: " + ReadInt(Compression, "CompressionCount"));
            sb.AppendLine("Shocks delivered: " + ReadInt(Defibrillator, "ShocksDelivered"));
            if (Tray != null) sb.Append(Tray.BuildSummary());
            return sb.ToString();
        }
    }
}