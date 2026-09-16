using UnityEngine;
using MedTriage.Simulation.Interactions;

namespace MedTriage.Simulation.Patients
{
    [DisallowMultipleComponent]
    public class PatientAnimationDriver : MonoBehaviour
    {
        public Animator TargetAnimator;
        public CompressionDetector Detector;
        public DefibrillatorController Defibrillator;
        public float CprHoldSeconds = 1.2f;
        public float JoltDuration = 0.18f;
        public float JoltHeightMeters = 0.035f;
        public AudioSource PatientVoice;
        public AudioClip RoscGroanClip;

        int _lastCount = -1;
        float _lastCompressionTime = -999f;
        int _lastShocks = -1;
        float _joltStart = -999f;
        Vector3 _basePos;
        bool _baseCaptured;
        bool _roscPlayed;

        public bool CprPlaying { get; private set; }

        void Awake()
        {
            if (TargetAnimator == null) TargetAnimator = GetComponentInChildren<Animator>(true);
            if (Detector == null) Detector = FindFirstObjectByType<CompressionDetector>(FindObjectsInactive.Include);
            if (Defibrillator == null) Defibrillator = FindFirstObjectByType<DefibrillatorController>(FindObjectsInactive.Include);
        }

        void Start()
        {
            if (TargetAnimator != null) { _basePos = TargetAnimator.transform.localPosition; _baseCaptured = true; }
            if (Detector != null) _lastCount = Detector.CompressionCount;
            if (Defibrillator != null) _lastShocks = Defibrillator.ShocksDelivered;
        }

        void Update()
        {
            if (TargetAnimator == null) return;
            bool cpr = false;
            if (Detector != null)
            {
                int c = Detector.CompressionCount;
                if (c != _lastCount) { _lastCount = c; _lastCompressionTime = Time.time; }
                cpr = Detector.HandsOnChest || (Time.time - _lastCompressionTime) < CprHoldSeconds;
            }
            CprPlaying = cpr;
            SetBool("CprActive", cpr);

            if (Defibrillator != null)
            {
                int s = Defibrillator.ShocksDelivered;
                if (_lastShocks >= 0 && s > _lastShocks) { _joltStart = Time.time; SetTrigger("ShockJolt"); }
                _lastShocks = s;
            }

            ApplyJolt();

            if (!_roscPlayed && PatientVoice != null && RoscGroanClip != null && Has("ROSC", AnimatorControllerParameterType.Bool) && TargetAnimator.GetBool("ROSC"))
            {
                _roscPlayed = true;
                PatientVoice.PlayOneShot(RoscGroanClip, 0.65f);
            }
        }

        void ApplyJolt()
        {
            if (!_baseCaptured) return;
            var tr = TargetAnimator.transform;
            float t = Time.time - _joltStart;
            if (t < 0f || t > JoltDuration)
            {
                tr.localPosition = Vector3.Lerp(tr.localPosition, _basePos, Time.deltaTime * 20f);
                return;
            }
            float k = Mathf.Sin((t / JoltDuration) * Mathf.PI);
            tr.localPosition = _basePos + Vector3.up * (JoltHeightMeters * k);
        }

        bool Has(string n, AnimatorControllerParameterType t)
        {
            if (TargetAnimator == null) return false;
            var ps = TargetAnimator.parameters;
            for (int i = 0; i < ps.Length; i++) if (ps[i].name == n && ps[i].type == t) return true;
            return false;
        }

        void SetBool(string n, bool v) { if (Has(n, AnimatorControllerParameterType.Bool)) TargetAnimator.SetBool(n, v); }
        void SetTrigger(string n) { if (Has(n, AnimatorControllerParameterType.Trigger)) TargetAnimator.SetTrigger(n); }
    }
}
