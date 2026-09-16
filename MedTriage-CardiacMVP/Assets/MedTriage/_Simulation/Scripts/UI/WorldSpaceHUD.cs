using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace MedTriage.Simulation.UI
{
    [DisallowMultipleComponent]
    public class WorldSpaceHUD : MonoBehaviour
    {
        public MonoBehaviour Compression;
        public MonoBehaviour Defibrillator;
        public MonoBehaviour TaskManagerRef;
        public MonoBehaviour CardiacInput;
        public Transform Anchor;
        public Vector3 LocalOffset = new Vector3(0f, -0.40f, 0.52f);
        public Vector3 LocalEuler = new Vector3(40f, 0f, 0f);
        public float PanelWidthMeters = 0.46f;
        public float PanelHeightMeters = 0.30f;
        public int FontPixels = 22;
        public bool Show = true;

        Canvas m_Canvas;
        Text m_Text;

        void Start()
        {
            if (Anchor == null && Camera.main != null) Anchor = Camera.main.transform;
            Build();
        }

        void Build()
        {
            var go = new GameObject("HUD Panel");
            go.transform.SetParent(Anchor, false);
            go.transform.localPosition = LocalOffset;
            go.transform.localRotation = Quaternion.Euler(LocalEuler);
            m_Canvas = go.AddComponent<Canvas>();
            m_Canvas.renderMode = RenderMode.WorldSpace;
            var rt = m_Canvas.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(460f, 300f);
            rt.localScale = new Vector3(PanelWidthMeters / 460f, PanelHeightMeters / 300f, 1f);
            var bg = new GameObject("BG");
            bg.transform.SetParent(go.transform, false);
            var img = bg.AddComponent<Image>();
            img.color = new Color(0f, 0f, 0f, 0.72f);
            var brt = img.GetComponent<RectTransform>();
            brt.anchorMin = Vector2.zero; brt.anchorMax = Vector2.one;
            brt.offsetMin = Vector2.zero; brt.offsetMax = Vector2.zero;
            var tgo = new GameObject("Text");
            tgo.transform.SetParent(go.transform, false);
            m_Text = tgo.AddComponent<Text>();
            var f = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (f == null) f = Resources.GetBuiltinResource<Font>("Arial.ttf");
            m_Text.font = f;
            m_Text.fontSize = FontPixels;
            m_Text.color = Color.white;
            m_Text.alignment = TextAnchor.UpperLeft;
            var trt = m_Text.GetComponent<RectTransform>();
            trt.anchorMin = Vector2.zero; trt.anchorMax = Vector2.one;
            trt.offsetMin = new Vector2(14f, 12f); trt.offsetMax = new Vector2(-14f, -12f);
        }

        object R(object o, string n)
        {
            if (o == null) return null;
            var t = o.GetType();
            var p = t.GetProperty(n, BindingFlags.Public | BindingFlags.Instance);
            if (p != null) return p.GetValue(o, null);
            var fi = t.GetField(n, BindingFlags.Public | BindingFlags.Instance);
            if (fi != null) return fi.GetValue(o);
            return null;
        }

        string S(object o, string n) { var v = R(o, n); return v == null ? "-" : v.ToString(); }
        float F(object o, string n) { var v = R(o, n); return v is float ? (float)v : 0f; }
        int I(object o, string n) { var v = R(o, n); return v is int ? (int)v : 0; }

        void Update()
        {
            if (m_Text == null || m_Canvas == null) return;
            m_Canvas.enabled = Show;
            if (!Show) return;
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("<b>MEDTRIAGE - CARDIAC ARREST</b>");
            sb.AppendLine("Session: " + S(TaskManagerRef, "SessionActive"));
            sb.AppendLine("Defib: " + S(Defibrillator, "State") + "   Shocks: " + S(Defibrillator, "ShocksDelivered"));
            sb.AppendLine("Charge: " + Mathf.RoundToInt(F(Defibrillator, "ChargeProgress01") * 100f) + "%   Deny: " + S(Defibrillator, "LastDenyReason"));
            sb.AppendLine("Hands locked: " + S(CardiacInput, "HandsLocked") + "   Placement: " + S(CardiacInput, "PlacementCorrect"));
            sb.AppendLine("Sternum dist: " + F(CardiacInput, "PlacementDistanceMeters").ToString("F3") + " m");
            sb.AppendLine("Compressions: " + S(Compression, "CompressionCount"));
            sb.AppendLine("Depth: " + (F(Compression, "CurrentDepthMeters") * 100f).ToString("F1") + " cm  (target 5-6)");
            sb.AppendLine("Rate: " + F(Compression, "CurrentRatePerMinute").ToString("F0") + " /min  (target 100-120)");
            m_Text.text = sb.ToString();
        }
    }
}