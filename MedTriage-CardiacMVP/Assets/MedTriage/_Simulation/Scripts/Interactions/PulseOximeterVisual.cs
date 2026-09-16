using UnityEngine;

namespace MedTriage.Simulation.Interactions
{
    [ExecuteAlways]
    public class PulseOximeterVisual : MonoBehaviour
    {
        public Color CasingColor = new Color(0.92f, 0.93f, 0.95f);
        public Color RubberGripColor = new Color(0.20f, 0.22f, 0.25f);
        public Color SensorLedColor = new Color(1.0f, 0.15f, 0.1f);
        public Color CableColor = new Color(0.35f, 0.38f, 0.42f);

        bool m_IsBuilding;

        void Awake()
        {
            HidePrimitiveRoot();
            if (Application.isPlaying) BuildVisual();
        }

        void Start()
        {
            HidePrimitiveRoot();
            if (transform.Find("Oximeter_Visual_Root") == null)
                BuildVisual();
        }

        void OnValidate()
        {
            // Only hide the placeholder mesh here. Do not create/destroy primitives in OnValidate.
            HidePrimitiveRoot();
        }

        void HidePrimitiveRoot()
        {
            var mr = GetComponent<MeshRenderer>();
            if (mr != null) mr.enabled = false;
        }

        public void BuildVisual()
        {
            if (m_IsBuilding) return;
            m_IsBuilding = true;
            HidePrimitiveRoot();

            var old = transform.Find("Oximeter_Visual_Root");
            if (old != null)
            {
                if (Application.isPlaying) Destroy(old.gameObject);
                else DestroyImmediate(old.gameObject);
            }

            var root = new GameObject("Oximeter_Visual_Root");
            root.transform.SetParent(transform, false);
            root.transform.localPosition = Vector3.zero;
            root.transform.localRotation = Quaternion.identity;

            var litShader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");

            MakeCube(root.transform, "Top_Jaw_Housing", new Vector3(0f, 0.008f, 0f), Quaternion.identity, new Vector3(0.024f, 0.008f, 0.052f), CasingColor, litShader, 0.65f, false);
            MakeCube(root.transform, "Bottom_Jaw_Housing", new Vector3(0f, -0.008f, 0f), Quaternion.identity, new Vector3(0.024f, 0.008f, 0.052f), CasingColor, litShader, 0.65f, false);
            MakeCube(root.transform, "Top_Silicone_Pad", new Vector3(0f, 0.0035f, 0.004f), Quaternion.identity, new Vector3(0.021f, 0.003f, 0.040f), RubberGripColor, litShader, 0.2f, false);
            MakeCube(root.transform, "Bottom_Silicone_Pad", new Vector3(0f, -0.0035f, 0.004f), Quaternion.identity, new Vector3(0.021f, 0.003f, 0.040f), RubberGripColor, litShader, 0.2f, false);
            MakeCylinder(root.transform, "Optical_Sensor_LED", new Vector3(0f, 0.0015f, 0.010f), Quaternion.identity, new Vector3(0.006f, 0.0015f, 0.006f), SensorLedColor, litShader, 0.5f, true);
            MakeCylinder(root.transform, "Spring_Hinge", new Vector3(0f, 0f, -0.022f), Quaternion.Euler(0f, 0f, 90f), new Vector3(0.012f, 0.011f, 0.012f), CasingColor, litShader, 0.65f, false);
            MakeCylinder(root.transform, "Cable_Strain_Relief", new Vector3(0f, -0.006f, -0.030f), Quaternion.Euler(90f, 0f, 0f), new Vector3(0.007f, 0.008f, 0.007f), RubberGripColor, litShader, 0.2f, false);
            MakeCylinder(root.transform, "Lead_Cable", new Vector3(0f, -0.012f, -0.060f), Quaternion.Euler(75f, 0f, 0f), new Vector3(0.004f, 0.028f, 0.004f), CableColor, litShader, 0.4f, false);

            m_IsBuilding = false;
        }

        void MakeCube(Transform parent, string name, Vector3 pos, Quaternion rot, Vector3 scale, Color color, Shader shader, float smoothness, bool emission)
        {
            Setup(GameObject.CreatePrimitive(PrimitiveType.Cube), parent, name, pos, rot, scale, color, shader, smoothness, emission);
        }

        void MakeCylinder(Transform parent, string name, Vector3 pos, Quaternion rot, Vector3 scale, Color color, Shader shader, float smoothness, bool emission)
        {
            Setup(GameObject.CreatePrimitive(PrimitiveType.Cylinder), parent, name, pos, rot, scale, color, shader, smoothness, emission);
        }

        void Setup(GameObject go, Transform parent, string name, Vector3 pos, Quaternion rot, Vector3 scale, Color color, Shader shader, float smoothness, bool emission)
        {
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            go.transform.localRotation = rot;
            go.transform.localScale = scale;

            var ren = go.GetComponent<MeshRenderer>();
            if (ren != null && shader != null)
            {
                var mat = new Material(shader);
                mat.color = color;
                if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", smoothness);
                if (emission)
                {
                    mat.EnableKeyword("_EMISSION");
                    if (mat.HasProperty("_EmissionColor")) mat.SetColor("_EmissionColor", color * 2.0f);
                }
                ren.sharedMaterial = mat;
            }

            var col = go.GetComponent<Collider>();
            if (col != null)
            {
                if (Application.isPlaying) Destroy(col);
                else DestroyImmediate(col);
            }
        }
    }
}