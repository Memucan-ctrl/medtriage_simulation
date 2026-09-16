using System;
using UnityEngine;
using UnityEngine.UI;

namespace MedTriage.Simulation.UI
{
    [DisallowMultipleComponent]
    public class DefibrillatorPanel : MonoBehaviour
    {
        public MonoBehaviour Defibrillator;
        public Transform Anchor;
        public Vector3 LocalOffset = new Vector3(0.25f, 1.15f, -2.85f);
        public Vector3 LocalEuler = Vector3.zero;
        public float WidthMeters = 0.36f;
        public float HeightMeters = 0.26f;
        public AudioClip ClickClip;
        public AudioClip ShockClip;

        public bool RhythmClassified { get; private set; }
        public bool ClassifiedCorrectly { get; private set; }

        const int PX_W = 440;
        const int PX_H = 320;
        Canvas m_Canvas;
        Text m_Status;
        Text m_Deny;
        Button m_Charge;
        Button m_Clear;
        Button m_Shock;
        Button m_Shockable;
        Button m_NonShockable;
        AudioSource m_Audio;
        Font m_Font;
        Transform m_Cam;

        void Start()
        {
            m_Audio = gameObject.AddComponent<AudioSource>();
            m_Audio.playOnAwake = false;
            m_Audio.spatialBlend = 0.7f;
            m_Font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (m_Font == null) m_Font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            Build();
        }

        void Build()
        {
            var go = new GameObject("DefibPanelCanvas");
            if (Anchor != null)
            {
                go.transform.SetParent(Anchor, false);
                go.transform.localPosition = LocalOffset;
                go.transform.localRotation = Quaternion.Euler(LocalEuler);
            }
            else
            {
                // Unparented world-space positioning directly over the foot of the bed
                go.transform.position = LocalOffset;
                go.transform.rotation = Quaternion.Euler(LocalEuler);
            }

            m_Canvas = go.AddComponent<Canvas>();
            m_Canvas.renderMode = RenderMode.WorldSpace;
            go.AddComponent<CanvasScaler>();
            go.AddComponent<GraphicRaycaster>();
            
            var tdgrType = FindType("UnityEngine.XR.Interaction.Toolkit.UI.TrackedDeviceGraphicRaycaster");
            if (tdgrType != null) go.AddComponent(tdgrType);

            var rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(PX_W, PX_H);
            rt.localScale = Vector3.one * (WidthMeters / PX_W);

            MakeImage(rt, new Vector2(0, 0), new Vector2(PX_W, PX_H), new Color(0.05f, 0.06f, 0.09f, 0.96f));
            MakeText(rt, new Vector2(0, 132), new Vector2(PX_W, 34), "DEFIBRILLATOR", 24, TextAnchor.MiddleCenter, Color.white);

            m_Shockable = MakeButton(rt, new Vector2(-108, 84), new Vector2(200, 46), "SHOCKABLE", new Color(0.55f, 0.14f, 0.14f));
            m_NonShockable = MakeButton(rt, new Vector2(108, 84), new Vector2(200, 46), "NON-SHOCKABLE", new Color(0.16f, 0.32f, 0.5f));
            m_Charge = MakeButton(rt, new Vector2(-140, 24), new Vector2(130, 46), "CHARGE", new Color(0.3f, 0.28f, 0.1f));
            m_Clear = MakeButton(rt, new Vector2(0, 24), new Vector2(130, 46), "CALL CLEAR", new Color(0.2f, 0.3f, 0.2f));
            m_Shock = MakeButton(rt, new Vector2(140, 24), new Vector2(130, 46), "SHOCK", new Color(0.6f, 0.2f, 0.05f));

            m_Status = MakeText(rt, new Vector2(0, -40), new Vector2(PX_W - 24, 60), "--", 20, TextAnchor.MiddleCenter, new Color(0.75f, 0.95f, 0.8f));
            m_Deny = MakeText(rt, new Vector2(0, -110), new Vector2(PX_W - 24, 60), "", 18, TextAnchor.MiddleCenter, new Color(1f, 0.65f, 0.35f));

            m_Shockable.onClick.AddListener(delegate { Classify(true); });
            m_NonShockable.onClick.AddListener(delegate { Classify(false); });
            m_Charge.onClick.AddListener(delegate { Call("BeginCharge"); });
            m_Clear.onClick.AddListener(delegate { Call("CallClear"); });
            m_Shock.onClick.AddListener(delegate { DoShock(); });
        }

        void Update()
        {
            if (m_Canvas == null) return;
            if (m_Cam == null && Camera.main != null) m_Cam = Camera.main.transform;
            if (m_Cam != null)
            {
                Vector3 toCam = m_Cam.position - m_Canvas.transform.position;
                toCam.y = 0f;
                if (toCam.sqrMagnitude > 0.001f)
                    m_Canvas.transform.rotation = Quaternion.LookRotation(toCam.normalized, Vector3.up);
            }
        }

        static Type FindType(string full)
        {
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                var t = a.GetType(full, false);
                if (t != null) return t;
            }
            return null;
        }

        Image MakeImage(RectTransform parent, Vector2 pos, Vector2 size, Color c)
        {
            var go = new GameObject("Img");
            go.transform.SetParent(parent, false);
            var img = go.AddComponent<Image>();
            img.color = c;
            var rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;
            return img;
        }

        Text MakeText(RectTransform parent, Vector2 pos, Vector2 size, string txt, int sz, TextAnchor align, Color col)
        {
            var go = new GameObject("Txt");
            go.transform.SetParent(parent, false);
            var t = go.AddComponent<Text>();
            t.font = m_Font;
            t.fontSize = sz;
            t.alignment = align;
            t.color = col;
            t.text = txt;
            var rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;
            return t;
        }

        Button MakeButton(RectTransform parent, Vector2 pos, Vector2 size, string label, Color bg)
        {
            var img = MakeImage(parent, pos, size, bg);
            var btn = img.gameObject.AddComponent<Button>();
            MakeText(img.rectTransform, Vector2.zero, size, label, 18, TextAnchor.MiddleCenter, Color.white);
            return btn;
        }

        void Classify(bool shockable)
        {
            RhythmClassified = true;
            ClassifiedCorrectly = shockable;
            if (m_Status != null) m_Status.text = shockable ? "CLASSIFIED: SHOCKABLE (VF)" : "CLASSIFIED: NON-SHOCKABLE";
            if (ClickClip != null && m_Audio != null) m_Audio.PlayOneShot(ClickClip, 0.7f);
        }

        void DoShock()
        {
            Call("DeliverShock");
            if (ShockClip != null && m_Audio != null) m_Audio.PlayOneShot(ShockClip, 0.9f);
        }

        void Call(string method)
        {
            if (Defibrillator != null)
                Defibrillator.SendMessage(method, SendMessageOptions.DontRequireReceiver);
            if (ClickClip != null && m_Audio != null) m_Audio.PlayOneShot(ClickClip, 0.7f);
        }
    }
}
