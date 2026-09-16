using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;

namespace MedTriage.Simulation.InputMapping
{
    // Drives this transform from the XR Hands Palm joint.
    // Falls back to FallbackTransform (e.g. a controller) when hands are not tracked,
    // so one transform serves both hand-tracking and controller modes.
    [DisallowMultipleComponent]
    public class PalmFollower : MonoBehaviour
    {
        public Handedness Hand = Handedness.Right;
        public Transform TrackingOrigin;
        public Transform FallbackTransform;

        public bool IsTracked { get; private set; }
        public bool UsingFallback { get; private set; }
        public Vector3 PalmPosition { get; private set; }

        static XRHandSubsystem s_Sub;
        static readonly List<XRHandSubsystem> s_Subs = new List<XRHandSubsystem>();

        static void EnsureSub()
        {
            if (s_Sub != null && s_Sub.running) return;
            s_Subs.Clear();
            SubsystemManager.GetSubsystems(s_Subs);
            s_Sub = null;
            for (int i = 0; i < s_Subs.Count; i++)
            {
                if (s_Subs[i] != null && s_Subs[i].running) { s_Sub = s_Subs[i]; return; }
            }
        }

        void Update()
        {
            IsTracked = false;
            UsingFallback = false;
            EnsureSub();
            if (s_Sub != null)
            {
                XRHand h = (Hand == Handedness.Left) ? s_Sub.leftHand : s_Sub.rightHand;
                if (h.isTracked)
                {
                    XRHandJoint j = h.GetJoint(XRHandJointID.Palm);
                    Pose pose;
                    if (j.TryGetPose(out pose))
                    {
                        Transform o = TrackingOrigin != null ? TrackingOrigin : transform.parent;
                        Vector3 wp = o != null ? o.TransformPoint(pose.position) : pose.position;
                        Quaternion wr = o != null ? o.rotation * pose.rotation : pose.rotation;
                        transform.SetPositionAndRotation(wp, wr);
                        PalmPosition = wp;
                        IsTracked = true;
                        return;
                    }
                }
            }
            if (FallbackTransform != null)
            {
                transform.SetPositionAndRotation(FallbackTransform.position, FallbackTransform.rotation);
                PalmPosition = FallbackTransform.position;
                UsingFallback = true;
            }
        }
    }
}