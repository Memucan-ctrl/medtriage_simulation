using UnityEngine;
using System.Reflection;

namespace MedTriage.Simulation.UI
{
    [DefaultExecutionOrder(9000)]
    public class CoachDirector : MonoBehaviour
    {
        public MonoBehaviour Monitor;
        public MonoBehaviour Compression;
        public MonoBehaviour Defibrillator;
        public MonoBehaviour Syringe;
        public int CompressionsRequired = 20;
        public int ShocksRequired = 2;
        public string LastText = "";

        object Get(MonoBehaviour m, string n)
        {
            if (m == null) return null;
            var t = m.GetType();
            var p = t.GetProperty(n, BindingFlags.Public | BindingFlags.Instance);
            if (p != null) { try { return p.GetValue(m, null); } catch { return null; } }
            var f = t.GetField(n, BindingFlags.Public | BindingFlags.Instance);
            if (f != null) { try { return f.GetValue(m); } catch { return null; } }
            return null;
        }

        int GetInt(MonoBehaviour m, string n)
        {
            object v = Get(m, n);
            if (v == null) return 0;
            try { return System.Convert.ToInt32(v); } catch { return 0; }
        }

        bool GetBool(MonoBehaviour m, string n)
        {
            object v = Get(m, n);
            if (v == null) return false;
            try { return System.Convert.ToBoolean(v); } catch { return false; }
        }

        string GetStr(MonoBehaviour m, string n)
        {
            object v = Get(m, n);
            return v == null ? "" : v.ToString();
        }

        void Update()
        {
            if (Monitor == null) return;

            string text;
            if (!GetBool(Monitor, "PowerOn"))
            {
                text = "";
            }
            else
            {
                string rhythm = GetStr(Monitor, "CurrentRhythm");
                int comps = GetInt(Compression, "CompressionCount");
                int shocks = GetInt(Defibrillator, "ShocksDelivered");
                bool epi = GetBool(Syringe, "Administered");
                string state = GetStr(Defibrillator, "State");

                bool arrest = rhythm.Contains("Fib") || rhythm.Contains("systole");

                if (!arrest)
                {
                    text = shocks > 0 ? "ROSC - PATIENT RECOVERED" : "MONITORING";
                }
                else
                {
                    string line1 = "CPR " + comps + "/" + CompressionsRequired +
                        "   SHOCK " + shocks + "/" + ShocksRequired +
                        "   EPI " + (epi ? "GIVEN" : "-");

                    string next;
                    if (state.Contains("Charging")) next = "CHARGING...";
                    else if (state.Contains("Charged") || state.Contains("Clear")) next = "PUT BOTH PADS ON THE CHEST";
                    else if (comps < CompressionsRequired) next = "COMPRESS THE CHEST";
                    else if (shocks < ShocksRequired) next = "PRESS CHARGE ON THE DEFIB";
                    else if (!epi) next = "GIVE EPINEPHRINE";
                    else next = "CONTINUE CPR";

                    text = line1 + "\n>> " + next;
                }
            }

            if (text != LastText)
            {
                LastText = text;
                var f = Monitor.GetType().GetField("Annotation", BindingFlags.Public | BindingFlags.Instance);
                if (f != null) { try { f.SetValue(Monitor, text); } catch { } }
            }
        }
    }
}
