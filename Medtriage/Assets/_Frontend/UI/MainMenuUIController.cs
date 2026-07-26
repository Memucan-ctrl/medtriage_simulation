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
            var completedTaskIds = await CloudSaveManager.LoadCompletedTaskIdsAsync();
            BuildGrid(completedTaskIds);
        }
 
        private void BuildGrid(System.Collections.Generic.List<string> completedTaskIds)
        {
            foreach (Transform child in gridParent)
                Destroy(child.gameObject);
 
            foreach (var entry in taskCatalog.Tasks)
            {
                var tile = Instantiate(tilePrefab, gridParent);
                bool isCompleted = completedTaskIds.Contains(entry.TaskId);
                tile.Setup(entry, isCompleted, OnTaskSelected);
            }
        }
 
        private void OnTaskSelected(TaskCatalogEntry entry)
        {
            SessionManager.Instance.LoadTask(entry.TaskId, entry.SceneName);
        }
    }
}