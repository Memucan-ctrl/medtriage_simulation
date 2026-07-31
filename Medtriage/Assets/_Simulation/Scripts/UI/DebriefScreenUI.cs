using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Medtriage.Shared.Data;
using Medtriage.Shared.Managers;

namespace Medtriage.Simulation.UI
{
    /// <summary>
    /// The end-of-session debrief from Section 8 of
    /// Medtriage_Cardiac_Arrest_Scenario_Guide.docx: composite score, category
    /// bars, a separate flagged-critical-errors list, and a coaching summary.
    /// By the time Show() is called, CardiacArrestSceneController has already
    /// saved the result via SessionManager - this screen only displays it and
    /// returns to the menu.
    /// </summary>
    public class DebriefScreenUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text compositeScoreLabel;
        [SerializeField] private TMP_Text needsReviewBanner;
        [SerializeField] private Transform categoryBarParent;
        [SerializeField] private GameObject categoryBarPrefab; // a small prefab: one TMP_Text + one Image fill bar
        [SerializeField] private Transform criticalErrorListParent;
        [SerializeField] private GameObject criticalErrorRowPrefab; // a simple TMP_Text row
        [SerializeField] private TMP_Text coachingSummaryLabel;
        [SerializeField] private Button returnToMenuButton;

        private void Awake()
        {
            if (returnToMenuButton != null)
                returnToMenuButton.onClick.AddListener(HandleReturnToMenu);
            else
                Debug.LogError("[DebriefScreenUI] returnToMenuButton is not assigned; the trainee will not be able to leave the debrief.", this);
        }

        private void OnDestroy()
        {
            if (returnToMenuButton != null)
                returnToMenuButton.onClick.RemoveListener(HandleReturnToMenu);
        }

        public void Show(TaskResult result)
        {
            gameObject.SetActive(true);

            if (result == null)
            {
                Debug.LogError("[DebriefScreenUI] Show called with a null TaskResult.", this);
                return;
            }

            if (compositeScoreLabel != null)
                compositeScoreLabel.text = string.Format("{0:0}", result.CompositeScore);

            if (needsReviewBanner != null)
            {
                needsReviewBanner.gameObject.SetActive(result.NeedsReview);
                if (result.NeedsReview)
                    needsReviewBanner.text = "Needs review - see flagged items below";
            }

            BuildCategoryBars(result.CategoryScores);
            BuildCriticalErrorList(result.FlaggedCriticalErrors);

            if (coachingSummaryLabel != null)
            {
                coachingSummaryLabel.text = string.IsNullOrEmpty(result.CoachingSummary)
                    ? "Coaching summary unavailable for this attempt."
                    : result.CoachingSummary;
            }
        }

        private void BuildCategoryBars(List<CategoryScore> scores)
        {
            if (categoryBarParent == null || categoryBarPrefab == null || scores == null) return;

            foreach (Transform child in categoryBarParent) Destroy(child.gameObject);

            foreach (CategoryScore score in scores)
            {
                GameObject row = Instantiate(categoryBarPrefab, categoryBarParent);
                TMP_Text label = row.GetComponentInChildren<TMP_Text>();
                Image fill = row.GetComponentInChildren<Image>();
                if (label != null) label.text = string.Format("{0}: {1:0}", score.Category, score.Score);
                if (fill != null) fill.fillAmount = Mathf.Clamp01(score.Score / 100f);
            }
        }

        private void BuildCriticalErrorList(List<string> errors)
        {
            if (criticalErrorListParent == null || criticalErrorRowPrefab == null || errors == null) return;

            foreach (Transform child in criticalErrorListParent) Destroy(child.gameObject);

            foreach (string error in errors)
            {
                GameObject row = Instantiate(criticalErrorRowPrefab, criticalErrorListParent);
                TMP_Text label = row.GetComponentInChildren<TMP_Text>();
                if (label != null) label.text = "\u26A0 " + error;
            }
        }

        private void HandleReturnToMenu()
        {
            if (SessionManager.Instance != null)
                SessionManager.Instance.ReturnToMenu();
            else
                Debug.LogWarning("[DebriefScreenUI] SessionManager is unavailable. Start the application from Bootstrap.", this);
        }
    }
}
