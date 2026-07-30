using System;
using UnityEngine;

namespace Medtriage.Shared.Data
{
    /// <summary>Stable critical-error identifier and clinically reviewed description.</summary>
    [Serializable]
    public class CriticalErrorRule
    {
        public string Id;
        [TextArea] public string Description;
    }
}
