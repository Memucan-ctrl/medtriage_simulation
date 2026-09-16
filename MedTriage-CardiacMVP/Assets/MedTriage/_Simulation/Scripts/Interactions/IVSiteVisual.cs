using UnityEngine;

namespace MedTriage.Simulation.Interactions
{
    [ExecuteAlways]
    public class IVSiteVisual : MonoBehaviour
    {
        public Color CannulaColor = new Color(0.12f, 0.58f, 0.98f);
        public Color DressingColor = new Color(0.95f, 0.95f, 0.98f, 0.85f);
        public Color PortColor = new Color(1.0f, 0.78f, 0.15f);

        bool m_IsBuilding;

        void Awake()
        {
            if (Application.isPlaying) BuildVisual();
        }

        void Start()
        {
            if (transform.Find("IV_Visual_Root") == null)
                BuildVisual();
        }

        void OnValidate()
        {
            // Do not create/destroy primitives during OnValidate; Unity forbids immediate destruction in this callback.
            var mr = GetComponent<MeshRenderer>();
            if (mr != null) mr.enabled = false;
        }

        public void BuildVisual()
        {
            if (m_IsBuilding) return;
            m_IsBuilding = true;

            var old = transform.Find("IV_Visual_Root");
            if (old != null)
            {
                if (Application.isPlaying) Destroy(old.gameObject);
                else DestroyImmediate(old.gameObject);
            }

            var root = new GameObject("IV_Visual_Root");
            root.transform.SetParent(transform, false);
            root.transform.localPosition = Vector3.zero;
            root.transform.localRotation = Quaternion.identity;

            Shader s = Shader.Find("Universal Render Pipeline/Lit")
                    ?? Shader.Find("Universal Render Pipeline/Simple Lit")
                    ?? Shader.Find("Mobile/Diffuse")
                    ?? Shader.Find("Standard");

            MakeCube(root.transform, "Dressing_Patch", new Vector3(0f, 0.003f, 0f), new Vector3(0.065f, 0.002f, 0.055f), DressingColor, s);
            MakeCube(root.transform, "Cannula_Hub", new Vector3(0f, 0.010f, 0f), new Vector3(0.014f, 0.010f, 0.028f), CannulaColor, s);
            MakeCube(root.transform, "Cannula_Wings", new Vector3(0f, 0.005f, -0.002f), new Vector3(0.040f, 0.003f, 0.012f), CannulaColor, s);
            MakeCylinder(root.transform, "Injection_Port", new Vector3(0f, 0.019f, -0.005f), Quaternion.identity, new Vector3(0.010f, 0.008f, 0.010f), PortColor, s);
            MakeCylinder(root.transform, "Extension_Line", new Vector3(-0.014f, 0.007f, 0.018f), Quaternion.Euler(0f, 45f, 90f), new Vector3(0.004f, 0.022f, 0.004f), new Color(0.9f, 0.9f, 0.95f, 0.85f), s);

            m_IsBuilding = false;
        }

        void MakeCube(Transform parent, string name, Vector3 pos, Vector3 scale, Color color, Shader shader)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Setup(go, parent, name, pos, Quaternion.identity, scale, color, shader);
        }

        void MakeCylinder(Transform parent, string name, Vector3 pos, Quaternion rot, Vector3 scale, Color color, Shader shader)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            Setup(go, parent, name, pos, rot, scale, color, shader);
        }

        void Setup(GameObject go, Transform parent, string name, Vector3 pos, Quaternion rot, Vector3 scale, Color color, Shader shader)
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