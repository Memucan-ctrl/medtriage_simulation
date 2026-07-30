using System;
using System.Text;
using System.Threading.Tasks;
using Medtriage.Shared.Data;
using UnityEngine;
using UnityEngine.Networking;

namespace Medtriage.Simulation.Networking
{
    [Serializable]
    public class DebriefResponse
    {
        public string summary;
        public string[] strengths;
        public string[] improvements;
        public string disclaimer;
    }

    /// <summary>Calls a secure backend proxy only. Never place a Gemini API key in Unity.</summary>
    public static class DebriefCoachingService
    {
        public static async Task<string> RequestSummaryAsync(string proxyUrl, TaskResult result, int timeoutSeconds = 10)
        {
            if (string.IsNullOrWhiteSpace(proxyUrl) || result == null) return null;

            byte[] body = Encoding.UTF8.GetBytes(JsonUtility.ToJson(result));
            using (var request = new UnityWebRequest(proxyUrl, UnityWebRequest.kHttpVerbPOST))
            {
                request.uploadHandler = new UploadHandlerRaw(body);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.timeout = Mathf.Max(1, timeoutSeconds);
                request.SetRequestHeader("Content-Type", "application/json");

                UnityWebRequestAsyncOperation operation = request.SendWebRequest();
                while (!operation.isDone) await Task.Yield();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogWarning($"[DebriefCoachingService] Proxy unavailable: {request.error}");
                    return null;
                }

                DebriefResponse response = JsonUtility.FromJson<DebriefResponse>(request.downloadHandler.text);
                return response != null && !string.IsNullOrWhiteSpace(response.summary) ? response.summary : null;
            }
        }

        public static string BuildLocalFallback(TaskResult result)
        {
            if (result == null) return "Debrief unavailable.";
            if (result.NeedsReview)
                return $"Score {result.CompositeScore:0}. Review the flagged safety items before repeating this scenario.";
            if (result.CompositeScore >= 85f)
                return $"Score {result.CompositeScore:0}. Strong overall performance. Repeat the scenario to improve speed and consistency.";
            return $"Score {result.CompositeScore:0}. Review the assessed categories and repeat the scenario to strengthen protocol adherence.";
        }
    }
}
