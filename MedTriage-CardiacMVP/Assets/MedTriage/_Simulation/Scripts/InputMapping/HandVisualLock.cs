using UnityEngine;

namespace MedTriage.Simulation.InputMapping
{
    // Failure #7 defence. Forces the hand visual to stay on its parent controller.
    // If anything at runtime displaces it, this snaps it back and logs the offset
    // so the cause is identifiable from adb logcat.
    [DefaultExecutionOrder(10000)]
    public class HandVisualLock : MonoBehaviour
    {
        public bool Lock = true;
        public Vector3 PositionOffset = Vector3.zero;
        public Vector3 RotationOffsetEuler = new Vector3(-35f, 0f, 0f);
        public bool LogDisplacement = true;
        public float LogIntervalSeconds = 1f;

        public float LastDisplacement { get; private set; }

        float m_Next;

        void LateUpdate()
        {
            float d = transform.localPosition.magnitude;
            LastDisplacement = d;

            if (LogDisplacement && Time.time >= m_Next)
            {
                m_Next = Time.time + Mathf.Max(0.25f, LogIntervalSeconds);
                Debug.Log("[MEDTRIAGE_HAND] " + name +
                    " localPos=" + transform.localPosition.ToString("F4") +
                    " mag=" + d.ToString("F4") +
                    " parent=" + (transform.parent != null ? transform.parent.name : "NULL"));
            }

            if (Lock)
            {
                transform.localPosition = PositionOffset;
                transform.localRotation = Quaternion.Euler(RotationOffsetEuler);
            }
        }
    }
}
