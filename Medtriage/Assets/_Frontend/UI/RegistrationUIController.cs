using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Medtriage.Frontend.Auth;
 
namespace Medtriage.Frontend.UI
{
    /// <summary>
    /// Drives the Registration scene: username/password/confirm-password fields and
    /// a Create Account button. See Medtriage_Team_Development_Guide.docx, Section 6.2.
    /// </summary>
    public class RegistrationUIController : MonoBehaviour
    {
        [Header("Fields")]
        [SerializeField] private TMP_InputField usernameField;
        [SerializeField] private TMP_InputField passwordField;
        [SerializeField] private TMP_InputField confirmPasswordField;
 
        [Header("Buttons")]
        [SerializeField] private Button createAccountButton;
        [SerializeField] private Button backToLoginButton;
 
        [Header("Feedback")]
        [SerializeField] private TMP_Text statusLabel;
        [SerializeField] private GameObject loadingSpinner;
 
        [Header("Navigation")]
        [SerializeField] private string loginSceneName = "Login";
 
        [Header("Validation")]
        [SerializeField] private int minPasswordLength = 8;
 
        private void Awake()
        {
            createAccountButton.onClick.AddListener(HandleCreateAccountClicked);
            backToLoginButton.onClick.AddListener(HandleBackToLoginClicked);
            SetBusy(false);
            SetStatus(string.Empty);
        }
 
        private async void HandleCreateAccountClicked()
        {
            var username = usernameField.text.Trim();
            var password = passwordField.text;
            var confirm = confirmPasswordField.text;
 
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                SetStatus("Enter a username and password.");
                return;
            }
            if (password.Length < minPasswordLength)
            {
                SetStatus($"Password must be at least {minPasswordLength} characters.");
                return;
            }
            if (password != confirm)
            {
                SetStatus("Passwords don't match.");
                return;
            }
 
            SetBusy(true);
            var result = await AuthManager.SignUpAsync(username, password);
            SetBusy(false);
 
            // On success, AuthManager already told SessionManager to load MainMenu,
            // so we only need to handle the failure case here.
            SetStatus(result.Success ? string.Empty : result.ErrorMessage);
        }
 
        private void HandleBackToLoginClicked()
        {
            SceneManager.LoadScene(loginSceneName);
        }
 
        private void SetBusy(bool busy)
        {
            createAccountButton.interactable = !busy;
            backToLoginButton.interactable = !busy;
            if (loadingSpinner != null) loadingSpinner.SetActive(busy);
        }
 
        private void SetStatus(string message)
        {
            if (statusLabel != null) statusLabel.text = message;
        }
    }
}
