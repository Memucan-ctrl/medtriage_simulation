using System.Threading.Tasks;
using UnityEngine;
 
namespace Medtriage.Shared.Managers
{
    /// <summary>
    /// A persistent full-screen fade used before/after every scene load, so scene
    /// transitions never feel abrupt in VR (see Medtriage_Team_Development_Guide.docx,
    /// Section 6.3). Put this on a Canvas + full-screen black Image in the Bootstrap
    /// scene, alongside SessionManager; it survives every scene load.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class SceneFadeTransition : MonoBehaviour
    {
        public static SceneFadeTransition Instance { get; private set; }
 
        [SerializeField] private float fadeDuration = 0.35f;
 
        private CanvasGroup canvasGroup;
 
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
 
            Instance = this;
            DontDestroyOnLoad(transform.root.gameObject);
 
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
        }
 
        public async Task FadeOutAsync()
        {
            canvasGroup.blocksRaycasts = true;
            await FadeToAsync(1f);
        }
 
        public async Task FadeInAsync()
        {
            await FadeToAsync(0f);
            canvasGroup.blocksRaycasts = false;
        }
 
        private async Task FadeToAsync(float target)
        {
            float start = canvasGroup.alpha;
            float t = 0f;
 
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(start, target, t / fadeDuration);
                await Task.Yield();
            }
 
            canvasGroup.alpha = target;
        }
    }
}