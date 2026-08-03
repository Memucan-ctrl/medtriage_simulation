namespace Medtriage.Simulation.Characters
{
    /// <summary>
    /// Delegable tasks from Section 6, Step 3.1 of
    /// Medtriage_Cardiac_Arrest_Scenario_Guide.docx.
    ///
    /// Explicit numeric values are pinned so that reordering or extending this enum
    /// later cannot silently remap serialized values already saved in scenes,
    /// prefabs or ScriptableObject assets.
    /// </summary>
    public enum TeammateTaskType
    {
        AttachMonitor = 0,
        GiveMedication = 1,
        GetIvAccess = 2,
        DocumentTime = 3
    }
}
