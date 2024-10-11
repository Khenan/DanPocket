using UnityEngine;

namespace Umeshu.Utility
{
    [System.Serializable]
    public struct FloatRange
    {
        public FloatRange(float _min, float _max)
        {
            this.min = _min;
            this.max = _max;
        }
        [SerializeField] private float min;
        [SerializeField] private float max;
        public float Min => min;
        public float Max => max;
        public float Center => (min + max) / 2;
        public float Range => max - min;
        public float GetValueAt(float _value) => min == max ? min : Mathf.Lerp(min, max, _value);
        public int GetIntValueAt(float _value) => Mathf.RoundToInt(min == max ? min : Mathf.Lerp(min, max, _value));
        public bool InRange(float _value) => min <= _value && max >= _value;
    }
}
