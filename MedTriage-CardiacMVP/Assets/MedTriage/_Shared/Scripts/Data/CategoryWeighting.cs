using System;

namespace MedTriage.Shared.Data
{
    [Serializable]
    public class CategoryWeighting
    {
        public ScoreCategory Category;
        public float Weight;
    }
}
