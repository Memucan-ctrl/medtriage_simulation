using UnityEngine;
using UnityEngine.SceneManagement;
 
namespace Medtriage.Frontend.Auth
{
    /// <summary>
    /// Attach to a GameObject in the Bootstrap scene, alongside SessionManager and
    /// SceneFadeTransition (Bootstrap is scene index 0 in Build Settings — see
    /// Medtriage_Team_Development_Guide.docx, Section 6.1). Initializes Unity Gaming
    /// Services, tries to resume a cached session, and routes to MainMenu or Login.
    /// </summary>
    public class AppBootstrapper : MonoBehaviour
    {
        [SerializeField] private string loginSceneName = "Login";
        [SerializeField] private string mainMenuSceneName = "MainMenu";
 
        private async void Start()
        {
            await AuthManager.InitializeServicesAsync();
            bool resumed = await AuthManager.TryResumeSessionAsync();
            SceneManager.LoadScene(resumed ? mainMenuSceneName : loginSceneName);
        }
    }
}
