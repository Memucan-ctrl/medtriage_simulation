using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MedTriage.Simulation.UI
{
    [DisallowMultipleComponent]
    public class MedicationTray : MonoBehaviour
    {
        [Serializable]
        public class DrugOption
        {
            public string Drug;
            public string Dose;
            public string Route;
            public bool Correct;
            public DrugOption(string d, string ds, string r, bool c) { Drug = d; Dose = ds; Route = r; Correct = c; }
            public string Label { get { return Drug + "\n" + Dose; } }
            public string Full { get { return Drug + " " + Dose + " " + Route; } }
        }

        public Transform Anchor;
        public Vector3 LocalOffset = new Vector3(0f, 1.05f, 0f);
        public Vector3 LocalEuler = Vector3.zero;
        public float WidthMeters = 0.52f;
        public float HeightMeters = 0.38f;
        public float EpinephrineIntervalSeconds = 180f;
        public AudioClip InjectionClip;
        public AudioClip SelectClip;
        // Quest VR hard fix: no floating medication UI panel. Medication is administered with the physical syringe only.
        public bool BuildVisualPanel = false;

        public int DosesGiven { get; private set; }
        public int CorrectDoses { get; private set; }
        public int WrongDoses { get; private set; }
        public string LastDrugGiven { get; private set; }
        public float LastEpinephrineTime { get; private set; }
        public bool EpinephrineGiven { get; private set; }
        public int AmiodaroneDoses { get; private set; }

        readonly List<DrugOption> m_Options = new List<DrugOption>();
        readonly List<string> m_Log = new List<string>();
        Canvas m_Canvas;
        Text m_Status;
        GameObject m_ConfirmPanel;
        Text m_ConfirmText;
        DrugOption m_Pending;
        AudioSource m_Audio;
        Font m_Font;

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
            if (Anchor == null) Anchor = transform;
            m_Font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (m_Font == null) m_Font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            m_Options.Add(new DrugOption("Epinephrine", "1 mg", "IV push", true));
            m_Options.Add(new DrugOption("Epinephrine", "10 mg", "IV push", false));
            m_Options.Add(new DrugOption("Amiodarone", "300 mg", "IV push", true));
            m_Options.Add(new DrugOption("Amiodarone", "150 mg", "IV push", true));
            m_Options.Add(new DrugOption("Lidocaine", "100 mg", "IV push", true));
            m_Options.Add(new DrugOption("Atropine", "1 mg", "IV push", false));
            m_Options.Add(new DrugOption("Sodium Bicarb", "50 mEq", "IV push", false));
            m_Options.Add(new DrugOption("Adenosine", "6 mg", "IV push", false));
            m_Audio = gameObject.AddComponent<AudioSource>();
            m_Audio.playOnAwake = false;
            m_Audio.spatialBlend = 0.8f;
            if (BuildVisualPanel) Build();
        }

        Text MakeText(GameObject parent, int size, Color col, TextAnchor anchor)
        {
            var go = new GameObject("Label");
            go.transform.SetParent(parent.transform, false);
            var t = go.AddComponent<Text>();
            t.font = m_Font;
            t.fontSize = size;
            t.color = col;
            t.alignment = anchor;
            t.horizontalOverflow = HorizontalWrapMode.Wrap;
            t.verticalOverflow = VerticalWrapMode.Overflow;
            var rt = t.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(4f, 2f); rt.offsetMax = new Vector2(-4f, -2f);
            return t;
        }

        void Build()
        {
            var go = new GameObject("Medication Tray");
            go.transform.SetParent(Anchor, false);
            go.transform.localPosition = LocalOffset;
            go.transform.localRotation = Quaternion.Euler(LocalEuler);
            m_Canvas = go.AddComponent<Canvas>();
            m_Canvas.renderMode = RenderMode.WorldSpace;
            go.AddComponent<GraphicRaycaster>();
            var tdgr = FindType("UnityEngine.XR.Interaction.Toolkit.UI.TrackedDeviceGraphicRaycaster");
            if (tdgr != null) go.AddComponent(tdgr);
            var rt = m_Canvas.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(520f, 380f);
            rt.localScale = new Vector3(WidthMeters / 520f, HeightMeters / 380f, 1f);

            var bg = new GameObject("BG");
            bg.transform.SetParent(go.transform, false);
            var img = bg.AddComponent<Image>();
            img.color = new Color(0.05f, 0.07f, 0.10f, 0.95f);
            var brt = img.GetComponent<RectTransform>();
            brt.anchorMin = Vector2.zero; brt.anchorMax = Vector2.one;
            brt.offsetMin = Vector2.zero; brt.offsetMax = Vector2.zero;

            var head = new GameObject("Header");
            head.transform.SetParent(go.transform, false);
            var hrt = head.AddComponent<RectTransform>();
            hrt.anchorMin = new Vector2(0f, 0.86f); hrt.anchorMax = new Vector2(1f, 1f);
            hrt.offsetMin = new Vector2(10f, 0f); hrt.offsetMax = new Vector2(-10f, -4f);
            MakeText(head, 26, new Color(0.9f, 0.95f, 1f), TextAnchor.MiddleLeft).text = "MEDICATION TRAY";

            int cols = 4;
            float cw = 1f / cols;
            for (int i = 0; i < m_Options.Count; i++)
            {
                var opt = m_Options[i];
                int cx = i % cols;
                int cy = i / cols;
                var bgo = new GameObject("Drug_" + opt.Drug + "_" + opt.Dose);
                bgo.transform.SetParent(go.transform, false);
                var bimg = bgo.AddComponent<Image>();
                bimg.color = new Color(0.16f, 0.20f, 0.26f, 1f);
                var btn = bgo.AddComponent<Button>();
                btn.targetGraphic = bimg;
                var cb = btn.colors;
                cb.highlightedColor = new Color(0.30f, 0.55f, 0.85f);
                cb.pressedColor = new Color(0.45f, 0.75f, 1f);
                btn.colors = cb;
                var brt2 = bimg.GetComponent<RectTransform>();
                brt2.anchorMin = new Vector2(cx * cw, 0.50f - cy * 0.18f);
                brt2.anchorMax = new Vector2((cx + 1) * cw, 0.66f - cy * 0.18f);
                brt2.offsetMin = new Vector2(6f, 4f); brt2.offsetMax = new Vector2(-6f, -4f);
                MakeText(bgo, 17, Color.white, TextAnchor.MiddleCenter).text = opt.Label;
                var captured = opt;
                btn.onClick.AddListener(delegate { OnPick(captured); });
            }

            var sgo = new GameObject("Status");
            sgo.transform.SetParent(go.transform, false);
            var srt = sgo.AddComponent<RectTransform>();
            srt.anchorMin = new Vector2(0f, 0f); srt.anchorMax = new Vector2(1f, 0.16f);
            srt.offsetMin = new Vector2(10f, 4f); srt.offsetMax = new Vector2(-10f, 0f);
            m_Status = MakeText(sgo, 19, new Color(0.75f, 0.85f, 0.95f), TextAnchor.MiddleLeft);

            BuildConfirm(go);
        }

        void BuildConfirm(GameObject parent)
        {
            m_ConfirmPanel = new GameObject("ConfirmPanel");
            m_ConfirmPanel.transform.SetParent(parent.transform, false);
            var img = m_ConfirmPanel.AddComponent<Image>();
            img.color = new Color(0.06f, 0.09f, 0.14f, 0.99f);
            var rt = img.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.06f, 0.18f); rt.anchorMax = new Vector2(0.94f, 0.84f);
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;

            var tgo = new GameObject("Question");
            tgo.transform.SetParent(m_ConfirmPanel.transform, false);
            var trt = tgo.AddComponent<RectTransform>();
            trt.anchorMin = new Vector2(0f, 0.45f); trt.anchorMax = new Vector2(1f, 1f);
            trt.offsetMin = new Vector2(12f, 0f); trt.offsetMax = new Vector2(-12f, -8f);
            m_ConfirmText = MakeText(tgo, 22, Color.white, TextAnchor.MiddleCenter);

            MakeButton(m_ConfirmPanel, "CONFIRM", new Color(0.12f, 0.45f, 0.20f), new Vector2(0.06f, 0.10f), new Vector2(0.47f, 0.40f), delegate { DoConfirm(); });
            MakeButton(m_ConfirmPanel, "CANCEL", new Color(0.45f, 0.15f, 0.15f), new Vector2(0.53f, 0.10f), new Vector2(0.94f, 0.40f), delegate { m_Pending = null; m_ConfirmPanel.SetActive(false); });
            m_ConfirmPanel.SetActive(false);
        }

        void MakeButton(GameObject parent, string label, Color col, Vector2 aMin, Vector2 aMax, UnityEngine.Events.UnityAction act)
        {
            var go = new GameObject("Btn_" + label);
            go.transform.SetParent(parent.transform, false);
            var img = go.AddComponent<Image>();
            img.color = col;
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;
            var rt = img.GetComponent<RectTransform>();
            rt.anchorMin = aMin; rt.anchorMax = aMax;
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
            MakeText(go, 20, Color.white, TextAnchor.MiddleCenter).text = label;
            btn.onClick.AddListener(act);
        }

        void OnPick(DrugOption opt)
        {
            m_Pending = opt;
            if (m_ConfirmText != null) m_ConfirmText.text = "Confirm: " + opt.Full + "?";
            if (m_ConfirmPanel != null) m_ConfirmPanel.SetActive(true);
            if (m_Audio != null && SelectClip != null) m_Audio.PlayOneShot(SelectClip, 0.4f);
        }

        // Records a real IV push from a physical prop (syringe / ampoule).
        // Same effect as confirming Epinephrine 1 mg IV push on the panel.
        public bool AdministerEpinephrineDirect() { return AdministerEpinephrinePhysical(); }
        public bool AdministerEpinephrinePhysical()
        {
            DosesGiven++;
            CorrectDoses++;
            LastDrugGiven = "Epinephrine 1 mg IV push";
            EpinephrineGiven = true;
            LastEpinephrineTime = Time.time;
            if (m_Log != null)
                m_Log.Add("medication_given|Epinephrine 1 mg IV push|correct=True|source=prop|t=" + Time.time.ToString("F1"));
            if (m_Audio != null && InjectionClip != null) m_Audio.PlayOneShot(InjectionClip, 0.8f);
            return true;
        }

        void DoConfirm()
        {
            if (m_Pending == null) { if (m_ConfirmPanel != null) m_ConfirmPanel.SetActive(false); return; }
            var opt = m_Pending;
            float interval = (opt.Drug == "Epinephrine" && EpinephrineGiven) ? (Time.time - LastEpinephrineTime) : -1f;
            DosesGiven++;
            if (opt.Correct) CorrectDoses++; else WrongDoses++;
            LastDrugGiven = opt.Full;
            if (opt.Drug == "Epinephrine" && opt.Correct)
            {
                EpinephrineGiven = true;
                LastEpinephrineTime = Time.time;
            }
            if (opt.Drug == "Amiodarone" && opt.Correct) AmiodaroneDoses++;
            m_Log.Add("medication_given|" + opt.Full + "|correct=" + opt.Correct + "|t=" + Time.time.ToString("F1") + "|interval=" + interval.ToString("F1"));
            if (m_Audio != null && InjectionClip != null) m_Audio.PlayOneShot(InjectionClip, 0.8f);
            m_Pending = null;
            if (m_ConfirmPanel != null) m_ConfirmPanel.SetActive(false);
        }

        public string BuildSummary()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("Doses given: " + DosesGiven + "  (correct " + CorrectDoses + ", wrong " + WrongDoses + ")");
            for (int i = 0; i < m_Log.Count; i++) sb.AppendLine(m_Log[i]);
            return sb.ToString();
        }

        public bool FacePlayer = false;
        Camera m_Cam;

        void Update()
        {
            if (FacePlayer && m_Canvas != null)
            {
                if (m_Cam == null) m_Cam = Camera.main;
                if (m_Cam != null)
                {
                    Vector3 toCam = m_Cam.transform.position - m_Canvas.transform.position;
                    toCam.y = 0f;
                    if (toCam.sqrMagnitude > 0.001f)
                    {
                        m_Canvas.transform.rotation = Quaternion.LookRotation(toCam);
                    }
                }
            }

            if (m_Status == null) return;
            string s;
            if (!EpinephrineGiven) s = "No epinephrine given yet";
            else
            {
                float due = EpinephrineIntervalSeconds - (Time.time - LastEpinephrineTime);
                if (due > 0f) s = "Next epi due in " + Mathf.FloorToInt(due / 60f) + ":" + Mathf.FloorToInt(due % 60f).ToString("00");
                else s = "<color=#FF6666>EPINEPHRINE DUE NOW</color>";
            }
            m_Status.text = s + "   |   Doses " + DosesGiven + "   Wrong " + WrongDoses;
        }
    }
}

