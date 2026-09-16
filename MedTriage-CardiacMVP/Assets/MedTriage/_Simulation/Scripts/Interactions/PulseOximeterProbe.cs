using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using MedTriage.Simulation.Patient;

namespace MedTriage.Simulation.Interactions
{
    public class PulseOximeterProbe : MonoBehaviour
    {
        public XRSocketInteractor Socket;
        public PatientPresence Presence;
        public AudioClip AttachClip;
        public float AttachVolume = 0.6f;
        [Tooltip("Exact scene object name of the only interactable allowed to activate monitoring.")]
        public string RequiredInteractableName = "PulseOximeter_Probe";

        AudioSource m_Audio;
        bool m_Attached;

        void Awake()
        {
            if (Socket == null) Socket = GetComponent<XRSocketInteractor>();
            if (Socket != null)
            {
                Socket.selectEntered.AddListener(OnSelectEntered);
                Socket.selectExited.AddListener(OnSelectExited);
            }

            m_Audio = GetComponent<AudioSource>();
            if (m_Audio == null) m_Audio = gameObject.AddComponent<AudioSource>();
            m_Audio.playOnAwake = false;
            m_Audio.spatialBlend = 1f;

            if (Presence == null) Presence = FindAnyObjectByType<PatientPresence>();
        }

        void OnDestroy()
        {
            if (Socket != null)
            {
                Socket.selectEntered.RemoveListener(OnSelectEntered);
                Socket.selectExited.RemoveListener(OnSelectExited);
            }
        }

        bool IsRequiredProbe(IXRSelectInteractable interactable)
        {
            if (interactable == null || interactable.transform == null) return false;

            Transform t = interactable.transform;
            if (t.name == RequiredInteractableName) return true;
            if (t.root != null && t.root.name == RequiredInteractableName) return true;
            if (t.GetComponentInParent<PulseOximeterVisual>() != null)
            {
                var visualRoot = t.GetComponentInParent<PulseOximeterVisual>().transform;
                return visualRoot.name == RequiredInteractableName || (visualRoot.root != null && visualRoot.root.name == RequiredInteractableName);
            }

            return false;
        }

        void OnSelectEntered(SelectEnterEventArgs args)
        {
            if (!IsRequiredProbe(args.interactableObject))
            {
                var rejectedName = args.interactableObject != null && args.interactableObject.transform != null
                    ? args.interactableObject.transform.name
                    : "<null>";
                Debug.LogWarning("[PulseOximeterProbe] Rejected non-matching object at finger socket: " + rejectedName);
                return;
            }

            if (m_Attached) return;
            m_Attached = true;

            if (m_Audio != null && AttachClip != null)
                m_Audio.PlayOneShot(AttachClip, AttachVolume);

            if (Presence != null)
            {
                Presence.OnOximeterAttached();
                Debug.Log("[PulseOximeterProbe] PulseOximeter_Probe attached to finger. Patient monitoring activated.");
            }
        }

        void OnSelectExited(SelectExitEventArgs args)
        {
            if (IsRequiredProbe(args.interactableObject))
                m_Attached = false;
        }
    }
}