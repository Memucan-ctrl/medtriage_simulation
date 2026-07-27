using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine;
using Medtriage.Shared.Data;
using Medtriage.Shared.Managers;
 
namespace Medtriage.Frontend.UI
{
    /// <summary>
    /// Populates the dashboard grid from a TaskCatalog asset and hands off to
    /// SessionManager.LoadTask when a trainee picks a scenario tile.
    /// See Medtriage_Team_Development_Guide.docx, Section 6.3.
    /// </summary>
    public class MainMenuUIController : MonoBehaviour
    {
        [SerializeField] private TaskCatalog taskCatalog;
        [SerializeField] private TaskTileButton tilePrefab;
        [SerializeField] private Transform gridParent;
 
private async void Start()
        {
            List<string> completedTaskIds = new List<string>();

            try
            {
                completedTaskIds = await CloudSaveManager.LoadCompletedTaskIdsAsync()
                    ?? new List<string>();
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"[MainMenu] Progress could not be loaded; scenarios remain available. {exception.Message}");
            }

            BuildGrid(completedTaskIds);
        }
 
private void BuildGrid(List<string> completedTaskIds)
        {
            if (gridParent == null || tilePrefab == null || taskCatalog == null)
            {
                Debug.LogError("[MainMenu] Scenario grid references are incomplete.");
                return;
            }

            foreach (Transform child in gridParent)
                Destroy(child.gameObject);

            if (taskCatalog.Tasks == null || taskCatalog.Tasks.Count == 0)
            {
                Debug.LogWarning("[MainMenu] The Task Catalog has no scenarios yet.");
                return;
            }

            foreach (TaskCatalogEntry entry in taskCatalog.Tasks)
            {
                if (entry == null || string.IsNullOrWhiteSpace(entry.TaskId))
                    continue;

                TaskTileButton tile = Instantiate(tilePrefab, gridParent);
                bool isCompleted = completedTaskIds.Contains(entry.TaskId);
                bool isAvailable = !string.IsNullOrWhiteSpace(entry.SceneName)
                    && Application.CanStreamedLevelBeLoaded(entry.SceneName);

                tile.Setup(entry, isCompleted, isAvailable, OnTaskSelected);
            }
        }
 
private void OnTaskSelected(TaskCatalogEntry entry)
        {
            if (entry == null || string.IsNullOrWhiteSpace(entry.SceneName))
            {
                Debug.LogWarning("[MainMenu] This scenario has no simulation scene assigned.");
                return;
            }

            if (!Application.CanStreamedLevelBeLoaded(entry.SceneName))
            {
                Debug.LogWarning($"[MainMenu] Scenario '{entry.DisplayName}' is waiting for scene '{entry.SceneName}' to be added to Build Settings.");
                return;
            }

            if (SessionManager.Instance == null)
            {
                Debug.LogWarning("[MainMenu] SessionManager is unavailable. Start the application from Bootstrap.");
                return;
            }

            SessionManager.Instance.LoadTask(entry.TaskId, entry.SceneName);
        }
    }
}