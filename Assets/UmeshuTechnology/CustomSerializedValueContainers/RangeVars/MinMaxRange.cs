using UnityEngine;

namespace Umeshu.Utility
{
    public class MinMaxRange : PropertyAttribute
    {
        public MinMaxRange(float _min, float _max)
        {
            this.min = _min;
            this.max = _max;
        }
        public readonly float min, max;
    }
}