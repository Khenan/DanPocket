using UnityEngine;

namespace Umeshu.Utility
{
    [System.Serializable]
    public struct IntRange
    {
        public IntRange(int _min, int _max)
        {
            this.min = _min;
            this.max = _max;
        }
        [SerializeField] private int min;
        [SerializeField] private int max;
        public int Min => min;
        public int Max => max;
        public int Center => (min + max) / 2;
        public int Range => max - min;
        public float GetValueAt(float _value) => min == max ? min : Mathf.Lerp(min, max, _value);
        public int GetIntValueAt(float _value) => min == max ? min : Mathf.RoundToInt(Mathf.Lerp(min, max, _value));
        public bool InRange(float _value) => min <= _value && max >= _value;
    }
}
