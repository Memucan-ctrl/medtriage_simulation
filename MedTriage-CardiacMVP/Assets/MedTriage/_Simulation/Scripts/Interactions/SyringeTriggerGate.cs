using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace MedTriage.Simulation.Interactions
{
    [DefaultExecutionOrder(-9000)]
    public class SyringeTriggerGate : MonoBehaviour
    {
        public MonoBehaviour Injector;
        public float WindowSeconds = 0.6f;
        public bool Armed;
        public string LastEvent = "";

        float m_Until;
        XRGrabInteractable m_Grab;

        void Awake()
        {
            if (Injector != null) Injector.enabled = true;
            m_Grab = GetComponent<XRGrabInteractable>();
            if (m_Grab != null) m_Grab.activated.AddListener(OnActivate);
        }

        void OnDestroy()
        {
            if (m_Grab != null) m_Grab.activated.RemoveListener(OnActivate);
        }

        void OnActivate(ActivateEventArgs a)
        {
            m_Until = Time.time + Mathf.Max(0.1f, WindowSeconds);
            LastEvent = "TRIGGER " + Time.time.ToString("F1");
        }

        void Update()
        {
            Armed = Time.time < m_Until;
            if (Injector != null && !Injector.enabled) Injector.enabled = true;
        }
    }
}
