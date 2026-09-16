using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MedTriage.Simulation.InputMapping
{
    [DisallowMultipleComponent]
    public class ControllerHandPoser : MonoBehaviour
    {
        public Transform Wrist;
        public bool IsRight = true;
        public float FingerCurlDegrees = 80f;
        public float ThumbCurlDegrees = 40f;
        public float SmoothSpeed = 16f;
        public float AngleSign = 1f;
        public bool DebugForceCurl = false;
        [Range(0f, 1f)] public float DebugCurlAmount = 0f;

        public float GripValue { get; private set; }
        public float TriggerValue { get; private set; }
        public int JointCount { get { return m_Joints.Count; } }

        class Joint
        {
            public Transform T;
            public Quaternion Open;
            public Vector3 Axis;
            public float Degrees;
            public bool UseTrigger;
        }

        readonly List<Joint> m_Joints = new List<Joint>();
        InputAction m_Grip;
        InputAction m_Trigger;

        void OnEnable()
        {
            string hand = IsRight ? "RightHand" : "LeftHand";
            m_Grip = new InputAction(binding: "<XRController>{" + hand + "}/grip", type: InputActionType.Value, expectedControlType: "Axis");
            m_Trigger = new InputAction(binding: "<XRController>{" + hand + "}/trigger", type: InputActionType.Value, expectedControlType: "Axis");
            m_Grip.Enable();
            m_Trigger.Enable();
            if (m_Joints.Count == 0) BuildJoints();
        }

        void OnDisable()
        {
            if (m_Grip != null) { m_Grip.Disable(); m_Grip.Dispose(); m_Grip = null; }
            if (m_Trigger != null) { m_Trigger.Disable(); m_Trigger.Dispose(); m_Trigger = null; }
        }

        public void BuildJoints()
        {
            m_Joints.Clear();
            if (Wrist == null) return;
            Vector3 palmNormal = Wrist.up;
            foreach (var t in Wrist.GetComponentsInChildren<Transform>(true))
            {
                string n = t.name;
                bool isSegment = n.Contains("Proximal") || n.Contains("Intermediate") || n.Contains("Distal");
                if (!isSegment) continue;
                if (t.childCount == 0) continue;
                Vector3 dir = (t.GetChild(0).position - t.position);
                if (dir.sqrMagnitude < 1e-10f) continue;
                Vector3 axisWorld = Vector3.Cross(dir.normalized, palmNormal);
                if (axisWorld.sqrMagnitude < 1e-8f) axisWorld = Wrist.right;
                bool isThumb = n.Contains("Thumb");
                var j = new Joint();
                j.T = t;
                j.Open = t.localRotation;
                j.Axis = t.InverseTransformDirection(axisWorld.normalized);
                j.Degrees = isThumb ? ThumbCurlDegrees : FingerCurlDegrees;
                j.UseTrigger = isThumb || n.Contains("Index");
                m_Joints.Add(j);
            }
        }

        public bool HoldingSyringe { get; set; }

        void LateUpdate()
        {
            GripValue = m_Grip != null ? Mathf.Clamp01(m_Grip.ReadValue<float>()) : 0f;
            TriggerValue = m_Trigger != null ? Mathf.Clamp01(m_Trigger.ReadValue<float>()) : 0f;
            if (DebugForceCurl) { GripValue = DebugCurlAmount; TriggerValue = DebugCurlAmount; }

            for (int i = 0; i < m_Joints.Count; i++)
            {
                var j = m_Joints[i];
                if (j.T == null) continue;

                float amount;
                if (HoldingSyringe)
                {
                    string n = j.T.name;
                    if (n.Contains("Thumb")) amount = Mathf.Lerp(0.25f, 0.7f, TriggerValue);
                    else if (n.Contains("Index") || n.Contains("Middle")) amount = 0.65f;
                    else amount = 0.85f;
                }
                else
                {
                    amount = j.UseTrigger ? TriggerValue : GripValue;
                }

                Quaternion target = j.Open * Quaternion.AngleAxis(amount * j.Degrees * AngleSign, j.Axis);
                j.T.localRotation = Quaternion.Slerp(j.T.localRotation, target, Time.deltaTime * SmoothSpeed);
            }
        }
    }
}