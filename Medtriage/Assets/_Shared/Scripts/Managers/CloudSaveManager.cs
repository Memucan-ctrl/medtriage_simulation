using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;
using UnityEngine;
using Medtriage.Shared.Data;
 
namespace Medtriage.Shared.Managers
{
    /// <summary>
    /// Thin wrapper around Unity Gaming Services Cloud Save. Stores each trainee's
    /// task results as Player Data, keyed by task id, plus a running list of
    /// completed task ids the MainMenu uses to show a "completed" badge on a tile.
    /// See Medtriage_Team_Development_Guide.docx, Section 6.2-6.3, and
    /// Medtriage_Cardiac_Arrest_Scenario_Guide.docx, Section 8.
    /// </summary>
    public static class CloudSaveManager
    {
        private const string CompletedTasksKey = "completed_task_ids";
        private const string ResultKeyPrefix = "result_";
 
        public static async Task SaveTaskResultAsync(TaskResult result)
        {
            try
            {
                var data = new Dictionary<string, object>
                {
                    { ResultKeyPrefix + result.TaskId, JsonUtility.ToJson(result) }
                };
                await CloudSaveService.Instance.Data.Player.SaveAsync(data);
                await AppendCompletedTaskIdAsync(result.TaskId);
            }
            catch (CloudSaveException e)
            {
                Debug.LogError($"[CloudSaveManager] Failed to save result for {result.TaskId}: {e.Message}");
            }
        }
 
        public static async Task<TaskResult> LoadTaskResultAsync(string taskId)
        {
            try
            {
                var key = ResultKeyPrefix + taskId;
                var loaded = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { key });
 
                if (loaded.TryGetValue(key, out var item))
                {
                    var json = item.Value.GetAs<string>();
                    return JsonUtility.FromJson<TaskResult>(json);
                }
 
                return null;
            }
            catch (CloudSaveException e)
            {
                Debug.LogError($"[CloudSaveManager] Failed to load result for {taskId}: {e.Message}");
                return null;
            }
        }
 
        public static async Task<List<string>> LoadCompletedTaskIdsAsync()
        {
            try
            {
                var loaded = await CloudSaveService.Instance.Data.Player.LoadAsync(
                    new HashSet<string> { CompletedTasksKey });
 
                if (loaded.TryGetValue(CompletedTasksKey, out var item))
                {
                    var json = item.Value.GetAs<string>();
                    var wrapper = JsonUtility.FromJson<StringListWrapper>(json);
                    return wrapper != null ? wrapper.items : new List<string>();
                }
 
                return new List<string>();
            }
            catch (CloudSaveException e)
            {
                Debug.LogError($"[CloudSaveManager] Failed to load completed task list: {e.Message}");
                return new List<string>();
            }
        }
 
        private static async Task AppendCompletedTaskIdAsync(string taskId)
        {
            var existing = await LoadCompletedTaskIdsAsync();
            if (!existing.Contains(taskId))
                existing.Add(taskId);
 
            var wrapper = new StringListWrapper { items = existing };
            var data = new Dictionary<string, object>
            {
                { CompletedTasksKey, JsonUtility.ToJson(wrapper) }
            };
 
            await CloudSaveService.Instance.Data.Player.SaveAsync(data);
        }
 
        [System.Serializable]
        private class StringListWrapper
        {
            public List<string> items = new List<string>();
        }
    }
}
