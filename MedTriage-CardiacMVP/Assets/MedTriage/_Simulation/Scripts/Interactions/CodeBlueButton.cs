using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using MedTriage.Simulation.Outcome;

namespace MedTriage.Simulation.Interactions
{
    [RequireComponent(typeof(XRSimpleInteractable))]
    [RequireComponent(typeof(AudioSource))]
    public class CodeBlueButton : MonoBehaviour
    {
        public Transform ButtonCap;
        public float PressDepth = 0.015f;
        public Color NormalColor = new Color(0.85f, 0.15f, 0.15f);
        public Color ActivatedColor = new Color(0.2f, 0.85f, 0.3f);
        public AudioClip AlertChime;
        public bool IsActivated { get; private set; }

        XRSimpleInteractable m_Interactable;
        AudioSource m_Audio;
        Vector3 m_InitialLocalPos;
        Material m_Mat;

        void Awake()
        {
            m_Interactable = GetComponent<XRSimpleInteractable>();
            m_Audio = GetComponent<AudioSource>();
            if (m_Audio == null) m_Audio = gameObject.AddComponent<AudioSource>();
            m_Audio.playOnAwake = false;
            m_Audio.spatialBlend = 0.8f;
            m_Audio.volume = 0.85f;

            if (ButtonCap == null) ButtonCap = transform.Find("Button_Cap") ?? transform;
            m_InitialLocalPos = ButtonCap.localPosition;

            var ren = ButtonCap.GetComponent<MeshRenderer>();
            if (ren != null) m_Mat = ren.material;

            if (m_Interactable != null)
            {
                m_Interactable.selectEntered.AddListener(OnPressed);
                m_Interactable.activated.AddListener(OnActivated);
                m_Interactable.hoverEntered.AddListener(OnHover);
            }
        }

        void OnHover(HoverEnterEventArgs args)
        {
            // Allow direct physical touch by controller
            if (args.interactorObject != null)
            {
                var interactorPos = args.interactorObject.transform.position;
                if (Vector3.Distance(interactorPos, transform.position) < 0.12f)
                {
                    PressButton();
                }
            }
        }

        void OnPressed(SelectEnterEventArgs args) { PressButton(); }
        void OnActivated(ActivateEventArgs args) { PressButton(); }

        public void PressButton()
        {
            if (IsActivated) return;
            IsActivated = true;

            // Depress button
            if (ButtonCap != null)
                ButtonCap.localPosition = m_InitialLocalPos - new Vector3(0f, 0f, PressDepth);

            // Change color
            if (m_Mat != null)
            {
                m_Mat.color = ActivatedColor;
                m_Mat.EnableKeyword("_EMISSION");
                m_Mat.SetColor("_EmissionColor", ActivatedColor * 1.5f);
            }

            // Play emergency chime
            if (AlertChime != null && m_Audio != null)
                m_Audio.PlayOneShot(AlertChime, 0.9f);

            // Notify ArrestLoopBridge
            var bridge = FindAnyObjectByType<ArrestLoopBridge>();
            if (bridge != null)
            {
                bridge.SendMessage("OnCodeBluePressed", SendMessageOptions.DontRequireReceiver);
            }

            Debug.Log("[CodeBlue] Code Blue emergency response team activated!");
        }
    }
}
