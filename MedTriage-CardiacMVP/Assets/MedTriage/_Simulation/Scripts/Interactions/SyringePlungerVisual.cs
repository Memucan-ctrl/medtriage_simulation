using UnityEngine;
using System.Reflection;

namespace MedTriage.Simulation.Interactions
{
    [DefaultExecutionOrder(9500)]
    public class SyringePlungerVisual : MonoBehaviour
    {
        public MonoBehaviour Injector;
        public Transform Plunger;
        public float DownTravel = 0.035f;
        public float TravelSeconds = 0.6f;
        public bool Fired;
        public float Progress;

        Vector3 m_Rest;
        bool m_Init;

        void Start()
        {
            if (Plunger != null)
            {
                m_Rest = Plunger.localPosition;
                m_Init = true;
            }
        }

        bool Administered()
        {
            if (Injector == null) return false;
            var p = Injector.GetType().GetProperty("Administered", BindingFlags.Public | BindingFlags.Instance);
            if (p == null) return false;
            object v = null;
            try { v = p.GetValue(Injector, null); } catch { return false; }
            if (v == null) return false;
            try { return System.Convert.ToBoolean(v); } catch { return false; }
        }

        void Update()
        {
            if (!m_Init || Plunger == null) return;

            if (!Fired && Administered()) Fired = true;
            if (!Fired) return;
            if (Progress >= 1f) return;

            Progress += Time.deltaTime / Mathf.Max(0.05f, TravelSeconds);
            if (Progress > 1f) Progress = 1f;

            Vector3 p2 = m_Rest;
            p2.y = m_Rest.y - DownTravel * Progress;
            Plunger.localPosition = p2;
        }
    }
}
