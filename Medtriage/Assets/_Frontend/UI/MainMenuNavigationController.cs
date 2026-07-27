using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Medtriage.Frontend.Auth;

namespace Medtriage.Frontend.UI
{
    /// <summary>Switches the MainMenu's panels while keeping scenario loading in MainMenuUIController.</summary>
    public sealed class MainMenuNavigationController : MonoBehaviour
    {
        [Header("Navigation")]
        [SerializeField] private Button homeButton;
        [SerializeField] private Button scenariosButton;
        [SerializeField] private Button progressButton;
        [SerializeField] private Button historyButton;
        [SerializeField] private Button guideButton;
        [SerializeField] private Button profileButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private Button signOutButton;

        [Header("Panels")]
        [SerializeField] private GameObject homePanel;
        [SerializeField] private GameObject scenariosPanel;
        [SerializeField] private GameObject progressPanel;
        [SerializeField] private GameObject historyPanel;
        [SerializeField] private GameObject guidePanel;
        [SerializeField] private GameObject profilePanel;
        [SerializeField] private GameObject settingsPanel;

        [Header("Header")]
        [SerializeField] private TMP_Text pageTitle;

        private void Awake()
        {
            Bind(homeButton, () => Show(homePanel, "Home"));
            Bind(scenariosButton, ShowScenarios);

            GameObject browseObject = GameObject.Find(
                "MainMenuCanvas/ContentArea/HomePanel/HeroCard/BrowseScenariosButton");
            Bind(browseObject != null ? browseObject.GetComponent<Button>() : null, ShowScenarios);

            Bind(progressButton, () => Show(progressPanel, "My Progress"));
            Bind(historyButton, () => Show(historyPanel, "Session History"));
            Bind(guideButton, () => Show(guidePanel, "User Guide"));
            Bind(profileButton, () => Show(profilePanel, "Profile"));
            Bind(settingsButton, () => Show(settingsPanel, "Settings"));
            Bind(signOutButton, AuthManager.SignOut);
            Bind(exitButton, ExitApplication);
            Show(homePanel, "Home");
        }

        private void ShowScenarios()
        {
            Show(scenariosPanel, "Scenarios");
        }

        private static void Bind(Button button, UnityEngine.Events.UnityAction action)
        {
            if (button == null) return;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(action);
        }

        private void Show(GameObject selected, string title)
        {
            GameObject[] panels =
            {
                homePanel, scenariosPanel, progressPanel, historyPanel,
                guidePanel, profilePanel, settingsPanel
            };

            foreach (GameObject panel in panels)
                if (panel != null) panel.SetActive(panel == selected);

            if (pageTitle != null)
                pageTitle.text = title;
        }

        private static void ExitApplication()
        {
#if UNITY_EDITOR
            Debug.Log("[MainMenu] Exit requested. Application.Quit only closes a player build.");
#else
            Application.Quit();
#endif
        }
    }
}
