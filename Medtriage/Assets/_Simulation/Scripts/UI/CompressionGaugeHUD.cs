using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Medtriage.Simulation.Interactions;

namespace Medtriage.Simulation.UI
{
    /// <summary>
    /// The live compression feedback HUD from Section 6, Step 2.1 of
    /// Medtriage_Cardiac_Arrest_Scenario_Guide.docx: a rate readout, a depth bar,
    /// a recoil indicator, and a hands-off warning. Assign a CompressionDetector
    /// from the scene in the Inspector.
    /// </summary>
    public class CompressionGaugeHUD : MonoBehaviour
    {
        [SerializeField] private CompressionDetector detector;
        [SerializeField] private TMP_Text rateLabel;
        [SerializeField] private Image depthBarFill;
        [SerializeField] private Image recoilIndicator;
        [SerializeField] private GameObject handsOffWarning;
        [SerializeField] private Color goodColor = Color.green;
        [SerializeField] private Color badColor = Color.red;

        [Header("Depth bar scale (meters at full bar)")]
        [SerializeField] private float depthBarMaxMeters = 0.06f;

        private bool subscribed;

        /// <summary>Allows a seeder or scene controller to bind the detector at runtime.</summary>
        public void Bind(CompressionDetector newDetector)
        {
            Unsubscribe();
            detector = newDetector;
            Subscribe();
        }

        private void OnEnable()
        {
            Subscribe();
            if (handsOffWarning != null) handsOffWarning.SetActive(false);
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            if (subscribed) return;

            if (detector == null)
            {
                Debug.LogError("[CompressionGaugeHUD] No CompressionDetector assigned; the compression HUD will not update. Assign it in the Inspector or call Bind().", this);
                return;
            }

            detector.OnCompressionSample += HandleSample;
            detector.OnHandsOffTooLong += HandleHandsOffTooLong;
            subscribed = true;
        }

        private void Unsubscribe()
        {
            if (!subscribed || detector == null) { subscribed = false; return; }

            detector.OnCompressionSample -= HandleSample;
            detector.OnHandsOffTooLong -= HandleHandsOffTooLong;
            subscribed = false;
        }

        private void HandleSample(float rate, float depth, bool goodRecoil)
        {
            if (handsOffWarning != null) handsOffWarning.SetActive(false);

            if (rateLabel != null)
            {
                rateLabel.text = string.Format("{0:0} /min", rate);
                rateLabel.color = detector.IsRateInRange(rate) ? goodColor : badColor;
            }

            if (depthBarFill != null)
            {
                float max = depthBarMaxMeters <= 0f ? 0.06f : depthBarMaxMeters;
                depthBarFill.fillAmount = Mathf.Clamp01(depth / max);
                depthBarFill.color = detector.IsDepthInRange(depth) ? goodColor : badColor;
            }

            if (recoilIndicator != null)
                recoilIndicator.color = goodRecoil ? goodColor : badColor;
        }

        private void HandleHandsOffTooLong(float seconds)
        {
            if (handsOffWarning != null) handsOffWarning.SetActive(true);
        }
    }
}
