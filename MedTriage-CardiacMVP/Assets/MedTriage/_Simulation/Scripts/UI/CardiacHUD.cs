using System.Reflection;
using UnityEngine;

namespace MedTriage.Simulation.UI
{
    public class CardiacHUD : MonoBehaviour
    {
        public MonoBehaviour Defibrillator;
        public MonoBehaviour Compression;
        public MonoBehaviour TaskManagerRef;
        public Transform SternumTarget;
        public Transform RightHandTransform;
        public float SternumRadiusMeters = 0.25f;
        public float SternumVerticalToleranceMeters = 0.25f;
        public bool Show = true;

        const BindingFlags BF = BindingFlags.Public | BindingFlags.Instance;

        static string R(object o, string n)
        {
            if (o == null) return "-";
            var t = o.GetType();
            var p = t.GetProperty(n, BF);
            if (p != null) { var v = p.GetValue(o); return v == null ? "-" : v.ToString(); }
            var f = t.GetField(n, BF);
            if (f != null) { var v = f.GetValue(o); return v == null ? "-" : v.ToString(); }
            return "?";
        }

        static float RF(object o, string n)
        {
            float x;
            return float.TryParse(R(o, n), out x) ? x : 0f;
        }

        void OnGUI()
        {
            if (!Show) return;
            float d = -1f;
            float vertical = 0f;
            if (SternumTarget != null && RightHandTransform != null)
            {
                Vector3 delta = RightHandTransform.position - SternumTarget.position;
                d = Vector3.ProjectOnPlane(delta, SternumTarget.up).magnitude;
                vertical = Mathf.Abs(Vector3.Dot(delta, SternumTarget.up));
            }
            GUI.Box(new Rect(430f, 10f, 400f, 200f), "CARDIAC STATUS");
            float y = 32f;
            GUI.Label(new Rect(440f, y, 380f, 20f), "Session active: " + R(TaskManagerRef, "SessionActive")); y += 20f;
            GUI.Label(new Rect(440f, y, 380f, 20f), "Defib: " + R(Defibrillator, "State") + "   shocks: " + R(Defibrillator, "ShocksDelivered")); y += 20f;
            float chg = RF(Defibrillator, "ChargeProgress01");
            GUI.Label(new Rect(440f, y, 380f, 20f), "Charge: " + Mathf.RoundToInt(chg * 100f) + "%   " + (chg >= 0.999f ? "READY - SHOCK NOW" : "charging...")); y += 20f;
            GUI.Label(new Rect(440f, y, 380f, 20f), "Last deny: " + R(Defibrillator, "LastDenyReason")); y += 20f;
            bool inRange = d >= 0f && d <= SternumRadiusMeters && vertical <= SternumVerticalToleranceMeters;
            GUI.Label(new Rect(440f, y, 380f, 20f), "Hand to chest: " + (d < 0f ? "n/a" : d.ToString("F2") + " m lat / " + vertical.ToString("F2") + " m vert") + (inRange ? "   IN RANGE" : "   TOO FAR")); y += 20f;
            GUI.Label(new Rect(440f, y, 380f, 20f), "Hands on chest: " + R(Compression, "HandsOnChest") + "   compressions: " + R(Compression, "CompressionCount")); y += 20f;
            GUI.Label(new Rect(440f, y, 380f, 20f), "Depth: " + (RF(Compression, "CurrentDepthMeters") * 100f).ToString("F1") + " cm  (target 5-6)"); y += 20f;
            GUI.Label(new Rect(440f, y, 380f, 20f), "Rate: " + RF(Compression, "CurrentRatePerMinute").ToString("F0") + " /min  (target 100-120)");
        }
    }
}