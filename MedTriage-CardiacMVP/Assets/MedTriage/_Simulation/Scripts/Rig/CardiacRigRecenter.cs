using System.Collections;
using UnityEngine;

namespace MedTriage.Simulation.RigSetup
{
    public class CardiacRigRecenter : MonoBehaviour
    {
        public Transform RigRoot;
        public Camera HeadCamera;
        public Transform LookTarget;
        public Vector3 StandPosition = new Vector3(0.62f, 0f, -3.60f);
        public bool RecenterOnStart = true;

        IEnumerator Start()
        {
            if (!RecenterOnStart) yield break;
            Recenter();
            yield return new WaitForSeconds(0.25f);
            Recenter();
            yield return new WaitForSeconds(0.5f);
            Recenter();
        }

        public void Recenter()
        {
            if (RigRoot == null || HeadCamera == null) return;
            Vector3 cam = HeadCamera.transform.position;
            RigRoot.position += new Vector3(StandPosition.x - cam.x, 0f, StandPosition.z - cam.z);
            if (LookTarget == null) return;
            Vector3 to = LookTarget.position - HeadCamera.transform.position;
            to.y = 0f;
            Vector3 fwd = HeadCamera.transform.forward;
            fwd.y = 0f;
            if (to.sqrMagnitude < 0.0001f || fwd.sqrMagnitude < 0.0001f) return;
            float yaw = Vector3.SignedAngle(fwd, to, Vector3.up);
            RigRoot.RotateAround(HeadCamera.transform.position, Vector3.up, yaw);
        }
    }
}