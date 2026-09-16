using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using MedTriage.Simulation.UI;

namespace MedTriage.Simulation.Interactions
{
    public enum SyringePhase
    {
        Empty,
        Drawing,
        Filled,
        Injecting,
        Administered
    }

    public class SyringeInjector : MonoBehaviour
    {
        public MedicationTray Tray;
        public Transform AmpouleTarget;
        public Transform IVSiteTarget;
        public Transform Plunger;

        public float AmpouleReachMeters = 0.28f;
        public float IVSiteReachMeters = 0.28f;
        public float PlungerEmptyY = 0.020f;
        public float PlungerFilledY = 0.055f;
        public float DrawDurationSeconds = 0.8f;
        public float InjectDurationSeconds = 0.6f;

        public AudioClip DrawClip;
        public AudioClip InjectClip;

        public SyringePhase Phase { get; private set; } = SyringePhase.Empty;
        public bool IsFilled => Phase == SyringePhase.Filled || Phase == SyringePhase.Injecting || Phase == SyringePhase.Administered;
        public bool Administered => Phase == SyringePhase.Administered;
        public float DrawProgress { get; private set; }
        public float InjectProgress { get; private set; }

        XRGrabInteractable m_Grab;
        AudioSource m_Audio;
        InputAction m_RightThumbstickY;
        InputAction m_RightTrigger;
        bool m_TriggerFired;

        void Awake()
        {
            m_Grab = GetComponent<XRGrabInteractable>();
            if (m_Grab != null)
            {
                m_Grab.activated.AddListener(OnActivated);
                m_Grab.selectEntered.AddListener(OnSelectEntered);
                m_Grab.selectExited.AddListener(OnSelectExited);
            }

            m_Audio = GetComponent<AudioSource>();
            if (m_Audio == null) m_Audio = gameObject.AddComponent<AudioSource>();
            m_Audio.playOnAwake = false;
            m_Audio.spatialBlend = 1f;
            m_Audio.rolloffMode = AudioRolloffMode.Linear;
            m_Audio.minDistance = 1f;
            m_Audio.maxDistance = 8f;
            m_Audio.volume = 0.40f;

            m_RightThumbstickY = new InputAction(binding: "<XRController>{RightHand}/thumbstick/y", type: InputActionType.Value, expectedControlType: "Axis");
            m_RightTrigger = new InputAction(binding: "<XRController>{RightHand}/trigger", type: InputActionType.Value, expectedControlType: "Axis");
        }

        void OnEnable()
        {
            m_RightThumbstickY?.Enable();
            m_RightTrigger?.Enable();
        }

        void OnDisable()
        {
            m_RightThumbstickY?.Disable();
            m_RightTrigger?.Disable();
        }

        void OnDestroy()
        {
            if (m_Grab != null)
            {
                m_Grab.activated.RemoveListener(OnActivated);
                m_Grab.selectEntered.RemoveListener(OnSelectEntered);
                m_Grab.selectExited.RemoveListener(OnSelectExited);
            }
            m_RightThumbstickY?.Dispose();
            m_RightTrigger?.Dispose();
        }

        MedTriage.Simulation.InputMapping.ControllerHandPoser m_CurrentPoser;

        void OnSelectEntered(SelectEnterEventArgs args)
        {
            if (args.interactorObject != null)
            {
                var p = args.interactorObject.transform.GetComponentInParent<MedTriage.Simulation.InputMapping.ControllerHandPoser>();
                if (p != null)
                {
                    m_CurrentPoser = p;
                    m_CurrentPoser.HoldingSyringe = true;
                }
            }
        }

        void OnSelectExited(SelectExitEventArgs args)
        {
            if (m_CurrentPoser != null)
            {
                m_CurrentPoser.HoldingSyringe = false;
                m_CurrentPoser = null;
            }
        }

        void OnActivated(ActivateEventArgs args)
        {
            m_TriggerFired = true;
        }

        void Start()
        {
            if (Tray == null) Tray = FindAnyObjectByType<MedicationTray>();

            if (AmpouleTarget == null)
            {
                var amp = GameObject.Find("Epinephrine_Ampoule");
                if (amp != null) AmpouleTarget = amp.transform;
            }

            if (IVSiteTarget == null)
            {
                var iv = GameObject.Find("IVSite_RightHand") ?? GameObject.Find("IVSite_RightAntecubital");
                if (iv != null) IVSiteTarget = iv.transform;
                else
                {
                    var torso = GameObject.Find("Patient_TorsoInteractable");
                    if (torso != null) IVSiteTarget = torso.transform;
                }
            }

            if (IVSiteTarget != null && IVSiteTarget.GetComponent<IVSiteVisual>() == null)
            {
                IVSiteTarget.gameObject.AddComponent<IVSiteVisual>();
            }

            if (Plunger == null)
            {
                var p = transform.Find("Plunger");
                if (p != null) Plunger = p;
            }

            SetPlungerY(PlungerEmptyY);
        }

        void SetPlungerY(float y)
        {
            if (Plunger == null) return;
            Vector3 lp = Plunger.localPosition;
            lp.y = y;
            Plunger.localPosition = lp;
        }

        void Update()
        {
            bool isHeld = m_Grab == null || m_Grab.isSelected;
            float stickY = m_RightThumbstickY != null ? m_RightThumbstickY.ReadValue<float>() : 0f;
            float triggerVal = m_RightTrigger != null ? m_RightTrigger.ReadValue<float>() : 0f;
            bool actionInput = stickY < -0.25f || triggerVal > 0.35f || m_TriggerFired;

            Vector3 pos = transform.position;

            // Phase 1 & 2: Empty -> Drawing -> Filled
            if (Phase == SyringePhase.Empty || Phase == SyringePhase.Drawing)
            {
                float ampDist = AmpouleTarget != null ? Vector3.Distance(pos, AmpouleTarget.position) : 999f;
                bool nearAmp = ampDist <= AmpouleReachMeters;

                if (isHeld && nearAmp && actionInput)
                {
                    if (Phase == SyringePhase.Empty)
                    {
                        Phase = SyringePhase.Drawing;
                        DrawProgress = 0f;
                        if (DrawClip != null && m_Audio != null) m_Audio.PlayOneShot(DrawClip, 0.7f);
                    }

                    DrawProgress += Time.deltaTime / Mathf.Max(0.1f, DrawDurationSeconds);
                    float curY = Mathf.Lerp(PlungerEmptyY, PlungerFilledY, Mathf.Clamp01(DrawProgress));
                    SetPlungerY(curY);

                    if (DrawProgress >= 1f)
                    {
                        Phase = SyringePhase.Filled;
                        SetPlungerY(PlungerFilledY);
                        Debug.Log("[Syringe] Successfully drawn 1 mg Epinephrine into barrel.");
                    }
                }
            }
            // Phase 3 & 4: Filled -> Injecting -> Administered
            else if (Phase == SyringePhase.Filled || Phase == SyringePhase.Injecting)
            {
                float ivDist = IVSiteTarget != null ? Vector3.Distance(pos, IVSiteTarget.position) : 999f;
                bool nearIV = ivDist <= IVSiteReachMeters;

                if (isHeld && nearIV && actionInput)
                {
                    if (Phase == SyringePhase.Filled)
                    {
                        Phase = SyringePhase.Injecting;
                        InjectProgress = 0f;
                        if (InjectClip != null && m_Audio != null) m_Audio.PlayOneShot(InjectClip, 0.7f);
                    }

                    InjectProgress += Time.deltaTime / Mathf.Max(0.1f, InjectDurationSeconds);
                    float curY = Mathf.Lerp(PlungerFilledY, PlungerEmptyY, Mathf.Clamp01(InjectProgress));
                    SetPlungerY(curY);

                    if (InjectProgress >= 1f)
                    {
                        Phase = SyringePhase.Administered;
                        SetPlungerY(PlungerEmptyY);
                        if (Tray != null) Tray.AdministerEpinephrineDirect();
                        Debug.Log("[Syringe] 1 mg Epinephrine injected at IV Cannula on hand!");
                    }
                }
            }

            m_TriggerFired = false;
        }
    }
}

