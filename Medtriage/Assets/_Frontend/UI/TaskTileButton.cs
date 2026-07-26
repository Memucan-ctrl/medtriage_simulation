using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Medtriage.Shared.Data;
 
namespace Medtriage.Frontend.UI
{
    /// <summary>
    /// One tile in the MainMenu's scenario grid. Populated at runtime by
    /// MainMenuUIController from a TaskCatalogEntry.
    /// See Medtriage_Team_Development_Guide.docx, Section 6.3.
    /// </summary>
    public class TaskTileButton : MonoBehaviour
    {
        [SerializeField] private TMP_Text titleLabel;
        [SerializeField] private TMP_Text descriptionLabel;
        [SerializeField] private Image thumbnailImage;
        [SerializeField] private Button startButton;
        [SerializeField] private GameObject completedBadge;
 
        private TaskCatalogEntry entry;
        private Action<TaskCatalogEntry> onSelected;
 
        public void Setup(TaskCatalogEntry entry, bool isCompleted, Action<TaskCatalogEntry> onSelected)
        {
            this.entry = entry;
            this.onSelected = onSelected;
 
            if (titleLabel != null) titleLabel.text = entry.DisplayName;
            if (descriptionLabel != null) descriptionLabel.text = entry.ShortDescription;
            if (thumbnailImage != null && entry.Thumbnail != null) thumbnailImage.sprite = entry.Thumbnail;
            if (completedBadge != null) completedBadge.SetActive(isCompleted);
 
            startButton.onClick.RemoveAllListeners();
            startButton.onClick.AddListener(HandleClicked);
        }
 
        private void HandleClicked() => onSelected?.Invoke(entry);
    }
}
