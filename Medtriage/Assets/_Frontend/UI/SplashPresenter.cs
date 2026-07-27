using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Medtriage.Frontend.UI
{
    /// <summary>Controls the branded Bootstrap presentation without owning authentication or routing.</summary>
    public sealed class SplashPresenter : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform logoRoot;
        [SerializeField] private TMP_Text statusLabel;
        [SerializeField, Min(0f)] private float minimumDisplaySeconds = 2.8f;
        [SerializeField, Min(0.1f)] private float revealSeconds = 0.8f;

        public void SetStatus(string message)
        {
            if (statusLabel != null)
                statusLabel.text = message;
        }

        public async Task PlayIntroAsync()
        {
            float startedAt = Time.realtimeSinceStartup;

            if (canvasGroup != null)
                canvasGroup.alpha = 0f;
            if (logoRoot != null)
                logoRoot.localScale = Vector3.one * 0.88f;

            float elapsed = 0f;
            while (elapsed < revealSeconds)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / revealSeconds);
                float eased = 1f - Mathf.Pow(1f - t, 3f);

                if (canvasGroup != null)
                    canvasGroup.alpha = eased;
                if (logoRoot != null)
                    logoRoot.localScale = Vector3.one * Mathf.Lerp(0.88f, 1f, eased);

                await Task.Yield();
            }

            while (Time.realtimeSinceStartup - startedAt < minimumDisplaySeconds)
                await Task.Yield();
        }
    }
}
