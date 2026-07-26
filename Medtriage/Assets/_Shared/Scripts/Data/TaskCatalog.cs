using System.Collections.Generic;
using UnityEngine;
 
namespace Medtriage.Shared.Data
{
    /// <summary>
    /// The list of scenarios shown as tiles on the MainMenu dashboard grid.
    /// Create one asset via Assets > Create > Medtriage > Task Catalog, then add one
    /// entry per Simulation scene. See Medtriage_Team_Development_Guide.docx,
    /// Section 6.3 and Section 8.
    /// </summary>
    [CreateAssetMenu(fileName = "TaskCatalog", menuName = "Medtriage/Task Catalog")]
    public class TaskCatalog : ScriptableObject
    {
        public List<TaskCatalogEntry> Tasks = new List<TaskCatalogEntry>();
    }
 
    [System.Serializable]
    public class TaskCatalogEntry
    {
        [Tooltip("Must match the Simulation team's ScenarioDefinition.scenarioId exactly.")]
        public string TaskId;
 
        [Tooltip("Shown on the dashboard tile, e.g. \"In-Hospital Cardiac Arrest\".")]
        public string DisplayName;
 
        [Tooltip("Must be added to File > Build Settings exactly as spelled here.")]
        public string SceneName;
 
        public Sprite Thumbnail;
 
        [TextArea]
        public string ShortDescription;
    }
}