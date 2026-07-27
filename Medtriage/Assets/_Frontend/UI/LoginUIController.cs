using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Medtriage.Frontend.Auth;
 
namespace Medtriage.Frontend.UI
{
    /// <summary>
    /// Drives the Login scene: a World Space canvas with username/password fields,
    /// a Log In button, and a link to the Registration scene.
    /// See Medtriage_Team_Development_Guide.docx, Section 6.2.
    /// </summary>
    public class LoginUIController : MonoBehaviour
    {
        [Header("Fields")]
        [SerializeField] private TMP_InputField usernameField;
        [SerializeField] private TMP_InputField passwordField;
 
        [Header("Buttons")]
        [SerializeField] private Button loginButton;
        
        [SerializeField] private Button continueAsTraineeButton;
[SerializeField] private Button goToRegistrationButton;
 
        [Header("Feedback")]
        [SerializeField] private TMP_Text statusLabel;
        [SerializeField] private GameObject loadingSpinner;
 
        [Header("Navigation")]
        [SerializeField] private string registrationSceneName = "Registration";
 
private void Awake()
        {
            loginButton.onClick.AddListener(HandleLoginClicked);
            goToRegistrationButton.onClick.AddListener(HandleGoToRegistrationClicked);
            if (continueAsTraineeButton != null)
                continueAsTraineeButton.onClick.AddListener(HandleContinueAsTraineeClicked);
            SetBusy(false);
            SetStatus(string.Empty);
        }
 
        private async void HandleLoginClicked()
        {
            var username = usernameField.text.Trim();
            var password = passwordField.text;
 
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                SetStatus("Enter a username and password.");
                return;
            }
 
            SetBusy(true);
            var result = await AuthManager.SignInAsync(username, password);
            SetBusy(false);
 
            // On success, AuthManager already told SessionManager to load MainMenu,
            // so we only need to handle the failure case here.
            SetStatus(result.Success ? string.Empty : result.ErrorMessage);
        }
 
        private void HandleGoToRegistrationClicked()
        {
            SceneManager.LoadScene(registrationSceneName);
        }
 
private void SetBusy(bool busy)
        {
            loginButton.interactable = !busy;
            goToRegistrationButton.interactable = !busy;
            if (continueAsTraineeButton != null)
                continueAsTraineeButton.interactable = !busy;
            if (loadingSpinner != null) loadingSpinner.SetActive(busy);
        }
 
        private void SetStatus(string message)
        {
            if (statusLabel != null) statusLabel.text = message;
        }
    

private async void HandleContinueAsTraineeClicked()
        {
            SetBusy(true);
            SetStatus("Creating your trainee session...");
            var result = await AuthManager.SignInAnonymouslyAsync();
            SetBusy(false);
            SetStatus(result.Success ? string.Empty : result.ErrorMessage);
        }
}
}
