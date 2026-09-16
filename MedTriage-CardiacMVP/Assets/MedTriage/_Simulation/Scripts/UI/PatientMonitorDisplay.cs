using System;
using UnityEngine;
using UnityEngine.UI;

namespace MedTriage.Simulation.UI
{
    [DisallowMultipleComponent]
    public class PatientMonitorDisplay : MonoBehaviour
    {
        public enum Rhythm { Asystole, VentricularFibrillation, SinusRhythm }

        public Rhythm CurrentRhythm = Rhythm.VentricularFibrillation;
        public Transform ScreenAnchor;
        public Vector3 LocalOffset = new Vector3(0f, 1.35f, 0.05f);
        public Vector3 LocalEuler = Vector3.zero;
        public float WidthMeters = 0.52f;
        public float HeightMeters = 0.34f;
        public AudioClip BeepClip;
        public AudioClip AlarmClip;
        public AudioClip BreathingClip;
        public bool PowerOn = true;
        public float AlarmIntervalSeconds = 3f;
        public float HeartRate = 82f;
        public float SpO2 = 97f;
        public float SweepColumnsPerSecond = 90f;

        public int DisplayHeartRate { get; private set; }

        const int W = 256;
        const int H = 72;
        Canvas m_Canvas;
        RawImage m_Trace;
        Text m_Readout;
    public string Annotation = "";
    string Ann() { return string.IsNullOrEmpty(Annotation) ? "" : "\n" + Annotation; }
        Texture2D m_Tex;
        AudioSource m_Audio;
        float m_Col;
        float m_Phase;
        float m_AlarmTimer;
        bool m_BeatFired;

        static Type FindType(string full)
        {
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                var t = a.GetType(full, false);
                if (t != null) return t;
            }
            return null;
        }

        void Start()
        {
            if (ScreenAnchor == null) ScreenAnchor = transform;
            Build();
        }

        void Build()
        {
            var go = new GameObject("Monitor Screen");
            go.transform.SetParent(ScreenAnchor, false);
            go.transform.localPosition = LocalOffset;
            go.transform.localRotation = Quaternion.Euler(LocalEuler);
            m_Canvas = go.AddComponent<Canvas>();
            m_Canvas.renderMode = RenderMode.WorldSpace;
            go.AddComponent<GraphicRaycaster>();
            var tdgr = FindType("UnityEngine.XR.Interaction.Toolkit.UI.TrackedDeviceGraphicRaycaster");
            if (tdgr != null) go.AddComponent(tdgr);
            var rt = m_Canvas.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(520f, 340f);
            rt.localScale = new Vector3(WidthMeters / 520f, HeightMeters / 340f, 1f);

            var bg = new GameObject("BG");
            bg.transform.SetParent(go.transform, false);
            var img = bg.AddComponent<Image>();
            img.color = new Color(0.02f, 0.03f, 0.02f, 1f);
            var brt = img.GetComponent<RectTransform>();
            brt.anchorMin = Vector2.zero; brt.anchorMax = Vector2.one;
            brt.offsetMin = Vector2.zero; brt.offsetMax = Vector2.zero;

            m_Tex = new Texture2D(W, H, TextureFormat.RGBA32, false);
            m_Tex.filterMode = FilterMode.Point;
            m_Tex.wrapMode = TextureWrapMode.Clamp;
            var clear = new Color32(4, 8, 4, 255);
            var px = new Color32[W * H];
            for (int i = 0; i < px.Length; i++) px[i] = clear;
            m_Tex.SetPixels32(px);
            m_Tex.Apply();

            var tgo = new GameObject("Trace");
            tgo.transform.SetParent(go.transform, false);
            m_Trace = tgo.AddComponent<RawImage>();
            m_Trace.texture = m_Tex;
            var trt = m_Trace.GetComponent<RectTransform>();
            trt.anchorMin = new Vector2(0f, 0.34f); trt.anchorMax = new Vector2(1f, 1f);
            trt.offsetMin = new Vector2(10f, 6f); trt.offsetMax = new Vector2(-10f, -10f);

            var rgo = new GameObject("Readout");
            rgo.transform.SetParent(go.transform, false);
            m_Readout = rgo.AddComponent<Text>();
            var f = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (f == null) f = Resources.GetBuiltinResource<Font>("Arial.ttf");
            m_Readout.font = f;
            m_Readout.fontSize = 30;
            m_Readout.color = new Color(0.35f, 1f, 0.45f);
            m_Readout.alignment = TextAnchor.UpperLeft;
            var rrt = m_Readout.GetComponent<RectTransform>();
            rrt.anchorMin = new Vector2(0f, 0f); rrt.anchorMax = new Vector2(1f, 0.34f);
            rrt.offsetMin = new Vector2(14f, 8f); rrt.offsetMax = new Vector2(-14f, -4f);

            m_Audio = gameObject.AddComponent<AudioSource>();
            m_Audio.playOnAwake = false;
            m_Audio.spatialBlend = 0.85f;
            m_Audio.minDistance = 1.5f;
            m_Audio.maxDistance = 12f;
        }

        public void SetRhythm(Rhythm r) { CurrentRhythm = r; }

        float Sample(float phase)
        {
            switch (CurrentRhythm)
            {
                case Rhythm.Asystole:
                    return UnityEngine.Random.Range(-0.02f, 0.02f);
                case Rhythm.VentricularFibrillation:
                    return Mathf.Sin(phase * 34f) * 0.28f + Mathf.Sin(phase * 11.3f) * 0.18f + UnityEngine.Random.Range(-0.12f, 0.12f);
                default:
                    float p = phase % 1f;
                    if (p < 0.06f) return Mathf.Sin(p / 0.06f * Mathf.PI) * 0.12f;
                    if (p < 0.10f) return -0.10f;
                    if (p < 0.14f) return 0.85f;
                    if (p < 0.18f) return -0.22f;
                    if (p < 0.34f) return Mathf.Sin((p - 0.18f) / 0.16f * Mathf.PI) * 0.16f;
                    return UnityEngine.Random.Range(-0.012f, 0.012f);
            }
        }

        void Update()
        {
            if (m_Tex == null || m_Readout == null) return;
            if (!PowerOn)
            {
                m_Readout.text = "MONITOR OFF" + Ann();
                return;
            }

            if (CurrentRhythm != Rhythm.SinusRhythm) HeartRate = 0f;
            else if (HeartRate < 20f) HeartRate = 82f;
            int rate = Mathf.RoundToInt(HeartRate);
            DisplayHeartRate = rate;
            float beatsPerSecond = CurrentRhythm == Rhythm.SinusRhythm ? Mathf.Max(20f, HeartRate) / 60f : 1.6f;
            m_Phase += Time.deltaTime * beatsPerSecond;

            float cols = Time.deltaTime * SweepColumnsPerSecond;
            int steps = Mathf.Clamp(Mathf.RoundToInt(cols), 1, 24);
            for (int s = 0; s < steps; s++)
            {
                m_Col += 1f;
                if (m_Col >= W) m_Col = 0f;
                int x = (int)m_Col;
                for (int y = 0; y < H; y++) m_Tex.SetPixel(x, y, new Color(0.016f, 0.031f, 0.016f));
                int gap = (x + 4) % W;
                for (int y = 0; y < H; y++) m_Tex.SetPixel(gap, y, new Color(0.016f, 0.031f, 0.016f));
                float v = Sample(m_Phase + s * 0.004f);
                int yy = Mathf.Clamp(Mathf.RoundToInt(H * 0.45f + v * H * 0.42f), 0, H - 1);
                m_Tex.SetPixel(x, yy, new Color(0.35f, 1f, 0.45f));
                if (yy + 1 < H) m_Tex.SetPixel(x, yy + 1, new Color(0.2f, 0.7f, 0.28f));
            }
            m_Tex.Apply(false);

            string label = CurrentRhythm == Rhythm.VentricularFibrillation ? "VENTRICULAR FIBRILLATION" : (CurrentRhythm == Rhythm.Asystole ? "ASYSTOLE" : "SINUS RHYTHM");
            string hr = rate > 0 ? rate.ToString() : "--";
            string spo2 = CurrentRhythm == Rhythm.SinusRhythm ? Mathf.RoundToInt(SpO2).ToString() : "--";
            string bp = CurrentRhythm == Rhythm.SinusRhythm ? "104/64" : "--/--";
            m_Readout.text = label + "\nHR " + hr + "    SpO2 " + spo2 + "%    BP " + bp + Ann();

            if (m_Audio == null) return;
            if (CurrentRhythm == Rhythm.SinusRhythm)
            {
                float p = m_Phase % 1f;
                if (p < 0.14f && !m_BeatFired)
                {
                    m_BeatFired = true;
                    if (BeepClip != null) m_Audio.PlayOneShot(BeepClip, 0.5f);
                }
                else if (p > 0.3f) m_BeatFired = false;
            }
            else if (CurrentRhythm == Rhythm.VentricularFibrillation)
            {
                m_AlarmTimer -= Time.deltaTime;
                if (m_AlarmTimer <= 0f)
                {
                    m_AlarmTimer = AlarmIntervalSeconds;
                    if (AlarmClip != null) m_Audio.PlayOneShot(AlarmClip, 0.55f);
                }
            }
        }
    }
}