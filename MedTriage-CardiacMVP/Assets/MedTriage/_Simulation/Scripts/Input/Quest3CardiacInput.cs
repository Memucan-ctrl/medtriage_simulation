using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using MedTriage.Simulation.Interactions;

namespace MedTriage.Simulation.InputMapping
{
    // Quest 3 Touch controller mapping for the cardiac arrest MVP.
    // Bindings are built in code against generic <XRController> usages so the SAME
    // mapping drives the XR Device Simulator on desktop AND the real Quest 3 headset.
    // No .inputactions asset is modified.
    public class Quest3CardiacInput : MonoBehaviour
    {
        [Header("Simulation refs")]
        public CompressionDetector Compression;
        public MonoBehaviour Defibrillator;
        public MonoBehaviour TaskManagerRef;
        public Transform SternumTarget;
        public Transform RightHandTransform;
        public PalmFollower LeftPalm;
        public PalmFollower RightPalm;

        [Header("Hand placement gate")]
        public float SternumRadiusMeters = 0.18f;
        public float PalmStackDistanceMeters = 0.14f;
        public float SternumVerticalToleranceMeters = 0.25f;
        public bool GateCompressions = true;
        public bool ShowOverlay = true;
        public bool AutoBeginSession = true;

        public bool HandsLocked { get; private set; }
        public bool PlacementCorrect { get; private set; }
        public float PlacementDistanceMeters { get; private set; }

        InputAction rGrip, rTrigger, rA, rB, lX, lY, lTrigger;
        string lastAction = "-";

        InputAction Make(string path)
        {
            var a = new InputAction(binding: path, type: InputActionType.Button);
            a.Enable();
            return a;
        }

        void OnEnable()
        {
            rGrip    = Make("<XRController>{RightHand}/gripPressed");
            rTrigger = Make("<XRController>{RightHand}/triggerPressed");
            rA       = Make("<XRController>{RightHand}/primaryButton");
            rB       = Make("<XRController>{RightHand}/secondaryButton");
            lX       = Make("<XRController>{LeftHand}/primaryButton");
            lY       = Make("<XRController>{LeftHand}/secondaryButton");
            lTrigger = Make("<XRController>{LeftHand}/triggerPressed");

            rA.performed       += _ => Fire(Defibrillator, "Attach pads", "AttachPads");
            rB.performed       += _ => Fire(Defibrillator, "Charge", "BeginCharge");
            rTrigger.performed += _ => Fire(Defibrillator, "Deliver shock", "TryDeliverShock");
            lX.performed       += _ => Fire(TaskManagerRef, "Begin session", "BeginSession");
            lY.performed       += _ => Rhythm();
            lTrigger.performed += _ => Fire(Defibrillator, "Call CLEAR", "CallClear");

            if (AutoBeginSession && TaskManagerRef != null)
                Fire(TaskManagerRef, "Auto-begin session", "BeginSession");
        }

        void OnDisable()
        {
            var all = new InputAction[] { rGrip, rTrigger, rA, rB, lX, lY, lTrigger };
            foreach (var a in all) { if (a != null) { a.Disable(); a.Dispose(); } }
        }

        void Update()
        {
            bool gripPressed = rGrip != null && rGrip.IsPressed();
            HandsLocked = gripPressed || BothPalmsStacked();

            Transform activeHand = ActiveHandTransform();
            if (SternumTarget != null && activeHand != null)
            {
                Vector3 d = activeHand.position - SternumTarget.position;
                PlacementDistanceMeters = Vector3.ProjectOnPlane(d, SternumTarget.up).magnitude;
                float vertical = Mathf.Abs(Vector3.Dot(d, SternumTarget.up));
                PlacementCorrect = PlacementDistanceMeters <= SternumRadiusMeters && vertical <= SternumVerticalToleranceMeters;
            }
            else
            {
                PlacementCorrect = false;
            }

            if (GateCompressions && Compression != null)
            {
                bool allow = HandsLocked && PlacementCorrect;
                if (Compression.enabled != allow) Compression.enabled = allow;
            }
        }

        public float PalmSeparationMeters { get; private set; }

        Transform ActiveHandTransform()
        {
            if (RightPalm != null && RightPalm.IsTracked) return RightPalm.transform;
            return RightHandTransform;
        }

        bool BothPalmsStacked()
        {
            if (LeftPalm == null || RightPalm == null) return false;
            if (!LeftPalm.IsTracked || !RightPalm.IsTracked) return false;
            PalmSeparationMeters = Vector3.Distance(LeftPalm.PalmPosition, RightPalm.PalmPosition);
            if (PalmSeparationMeters > PalmStackDistanceMeters) return false;
            if (SternumTarget == null) return true;
            Vector3 d = RightPalm.PalmPosition - SternumTarget.position;
            if (Mathf.Abs(Vector3.Dot(d, SternumTarget.up)) > SternumVerticalToleranceMeters) return false;
            return Vector3.ProjectOnPlane(d, SternumTarget.up).magnitude <= SternumRadiusMeters;
        }

        void Fire(MonoBehaviour target, string label, string method)
        {
            lastAction = label;
            if (target == null) { Debug.LogWarning("[Quest3Input] No target assigned for " + method); return; }
            var mi = target.GetType().GetMethod(method, BindingFlags.Public | BindingFlags.Instance);
            if (mi == null) { Debug.LogWarning("[Quest3Input] Method not found: " + method + " on " + target.GetType().Name); return; }
            try { mi.Invoke(target, mi.GetParameters().Length == 0 ? null : new object[mi.GetParameters().Length]); }
            catch (Exception e) { Debug.LogWarning("[Quest3Input] " + method + " failed: " + e.Message); }
        }

        void Rhythm()
        {
            lastAction = "Analyze rhythm";
            if (Defibrillator == null) { Debug.LogWarning("[Quest3Input] No defibrillator assigned"); return; }
            var mi = Defibrillator.GetType().GetMethod("ClassifyRhythm", BindingFlags.Public | BindingFlags.Instance);
            if (mi == null) { Debug.LogWarning("[Quest3Input] ClassifyRhythm not found"); return; }
            var ps = mi.GetParameters();
            try
            {
                if (ps.Length == 1 && ps[0].ParameterType.IsEnum)
                    mi.Invoke(Defibrillator, new object[] { Enum.Parse(ps[0].ParameterType, "Shockable") });
                else
                    mi.Invoke(Defibrillator, null);
            }
            catch (Exception e) { Debug.LogWarning("[Quest3Input] ClassifyRhythm failed: " + e.Message); }
        }

        void OnGUI()
        {
            if (!ShowOverlay) return;
            GUI.Box(new Rect(10, 210, 400, 232), "QUEST 3 TOUCH - CPR CONTROLS");
            int y = 235;
            GUI.Label(new Rect(20, y, 380, 20), "R Grip (HOLD) = interlock hands on chest"); y += 20;
            GUI.Label(new Rect(20, y, 380, 20), "R Trigger      = deliver shock"); y += 20;
            GUI.Label(new Rect(20, y, 380, 20), "A              = attach pads"); y += 20;
            GUI.Label(new Rect(20, y, 380, 20), "B              = charge defibrillator"); y += 20;
            GUI.Label(new Rect(20, y, 380, 20), "X              = begin session"); y += 20;
            GUI.Label(new Rect(20, y, 380, 20), "Y              = analyze rhythm"); y += 20;
            GUI.Label(new Rect(20, y, 380, 20), "L Trigger      = call CLEAR"); y += 24;
            GUI.Label(new Rect(20, y, 380, 20), "Hands locked: " + HandsLocked + "   Placement OK: " + PlacementCorrect); y += 20;
            GUI.Label(new Rect(20, y, 380, 20), "Sternum dist: " + PlacementDistanceMeters.ToString("F3") + " m"); y += 20;
            int cc = Compression != null ? Compression.CompressionCount : 0;
            GUI.Label(new Rect(20, y, 380, 20), "Compressions: " + cc + "   Last: " + lastAction);
        }
    }
}