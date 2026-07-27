using UnityEngine;

namespace Medtriage.Frontend.UI
{
    /// <summary>
    /// Continuously rotates the RectTransform to create a visible spinning/loading
    /// animation. Attach to a GameObject that has a circular or arc Image child.
    /// The existing LoadingSpinner GameObjects (activated/deactivated by
    /// LoginUIController and RegistrationUIController via SetActive) will pick this
    /// up automatically when the parent is enabled.
    /// </summary>
    public class UISpinner : MonoBehaviour
    {
        [SerializeField] private float degreesPerSecond = 360f;

        private RectTransform rect;

        private void Awake()
        {
            rect = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (rect != null)
                rect.Rotate(0f, 0f, -degreesPerSecond * Time.deltaTime);
        }
    }
}
