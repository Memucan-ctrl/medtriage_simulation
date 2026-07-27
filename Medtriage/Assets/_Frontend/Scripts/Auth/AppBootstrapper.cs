using UnityEngine;

using System.Threading.Tasks;
using Medtriage.Shared.Managers;
using Medtriage.Frontend.UI;
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
        
        [SerializeField] private SplashPresenter splashPresenter;
[SerializeField] private string mainMenuSceneName = "MainMenu";
 
private async void Start()
        {
            splashPresenter?.SetStatus("Initializing secure services...");
            Task introTask = splashPresenter != null
                ? splashPresenter.PlayIntroAsync()
                : Task.CompletedTask;

            await AuthManager.InitializeServicesAsync();
            splashPresenter?.SetStatus("Restoring trainee session...");

            bool resumed = await AuthManager.TryResumeSessionAsync();
            splashPresenter?.SetStatus(resumed ? "Preparing your dashboard..." : "Ready to begin...");
            await introTask;

            if (resumed)
                SessionManager.Instance?.OnLoginSuccess(AuthManager.CurrentPlayerId);
            else
                SceneManager.LoadScene(loginSceneName);
        }
    }
}
