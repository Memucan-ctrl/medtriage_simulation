using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Medtriage.Shared.Data;
 
namespace Medtriage.Shared.Managers
{
    /// <summary>
    /// The always-on "hallway" between the Frontend and the Simulation scenes.
    /// Lives in the Bootstrap scene and survives every scene load. Tracks who is
    /// signed in and which task was chosen, and is the single place both teams'
    /// scenes call into to move between each other.
    /// See Medtriage_Team_Development_Guide.docx, Section 2 and Section 6.
    /// </summary>
    public class SessionManager : MonoBehaviour
    {
        public static SessionManager Instance { get; private set; }
 
        [Header("Scene names (must match Build Settings exactly)")]
        [SerializeField] private string mainMenuSceneName = "MainMenu";
        [SerializeField] private string loginSceneName = "Login";
 
        public bool IsSignedIn { get; private set; }
        public string CurrentPlayerId { get; private set; }
        public string CurrentTaskId { get; private set; }
 
        public event Action OnSignedIn;
        public event Action OnSignedOut;
 
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
 
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
 
        /// <summary>Called by AuthManager once Unity Gaming Services Authentication succeeds.</summary>
        public void OnLoginSuccess(string playerId)
        {
            CurrentPlayerId = playerId;
            IsSignedIn = true;
            OnSignedIn?.Invoke();
            _ = LoadSceneWithFadeAsync(mainMenuSceneName);
        }
 
        public void SignOut()
        {
            CurrentPlayerId = null;
            CurrentTaskId = null;
            IsSignedIn = false;
            OnSignedOut?.Invoke();
            _ = LoadSceneWithFadeAsync(loginSceneName);
        }
 
        /// <summary>Called by MainMenuUIController when the trainee picks a scenario tile.</summary>
        public void LoadTask(string taskId, string sceneName)
        {
            if (!IsSignedIn)
            {
                Debug.LogWarning("[SessionManager] LoadTask called before sign-in; ignoring.");
                return;
            }
 
            CurrentTaskId = taskId;
            _ = LoadSceneWithFadeAsync(sceneName);
        }
 
        /// <summary>Called by a Simulation scene's exit/return control.</summary>
        public void ReturnToMenu()
        {
            CurrentTaskId = null;
            _ = LoadSceneWithFadeAsync(mainMenuSceneName);
        }
 
        /// <summary>
        /// Called at the end of a scenario with the finished, already-graded result
        /// (see TaskResult.cs). Saves it via Unity Gaming Services Cloud Save.
        /// </summary>
        public async Task RecordProgressAsync(TaskResult result)
        {
            if (!IsSignedIn)
            {
                Debug.LogWarning("[SessionManager] RecordProgressAsync called before sign-in; result was not saved.");
                return;
            }
 
            await CloudSaveManager.SaveTaskResultAsync(result);
        }
 
        private async Task LoadSceneWithFadeAsync(string sceneName)
        {
            if (SceneFadeTransition.Instance != null)
                await SceneFadeTransition.Instance.FadeOutAsync();
 
            var op = SceneManager.LoadSceneAsync(sceneName);
            while (op != null && !op.isDone)
                await Task.Yield();
 
            if (SceneFadeTransition.Instance != null)
                await SceneFadeTransition.Instance.FadeInAsync();
        }
    }
}
