using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Umeshu.Utility
{
    [System.Serializable]
    public class Distribution<T> : IList<T>
    {
        #region  Constructor
        public Distribution()
        {
        }
        public Distribution(int _capacity)
        {
            for (int _i = 0; _i < _capacity; _i++)
                Add(default);
        }
        public Distribution(List<T> _associations)
        {
            for (int _i = 0; _i < association.Capacity; _i++)
                Add(_associations[_i]);
        }
        #endregion
        private const float MAX = 1;
        [SerializeField]
        private List<float> maxCursors = new();
        [SerializeField]
        private List<T> association = new();


        #region  Accessors
        public bool IsReadOnly => false;
        public int Count => association.Count;
        public T this[int _index] { get => association[_index]; set => association[_index] = value; }
        public int CursorCount => maxCursors.Count;
        public IEnumerable<float> MaxCursors => maxCursors;
        #endregion

        #region StaticMethods
        public static List<float> GetEvenDistribution(FloatRange _range, int _count) => GetEvenDistribution(_range.Min, _range.Max, _count);
        public static List<float> GetEvenDistribution(float _min, float _max, int _count)
        {
            List<float> _subDivision = new();
            float _step = GetEvenStep(_count);
            float _value = _min + _step;
            for (int _i = 1; _i < _count; _i++)
            {
                _subDivision.Add(_value);
                _value += _step;
            }
            return _subDivision;
        }
        private static float GetStep(int _count, float _min, float _max) => (_max - _min) / _count;
        private static float GetEvenStep(int _count) => MAX / _count;
        #endregion StaticMethods

        #region  Collection
        public T ValueAt(float _cursorValue)
        {
            int _index = IndexAt(_cursorValue);
            return _index == -1 ? default : association[_index];
        }
        public int IndexAt(float _cursorValue)
        {
            Debug.Log(_cursorValue);
            if (association?.Count == 0) return -1;
            for (int _i = CursorCount - 1; _i > -1; _i--)
            {
                if (maxCursors[_i] < _cursorValue)
                    return _i + 1;
            }
            return 0;
        }
        public float SizeAt(int _index)
        {
            float _last = _index > 0 ? maxCursors[_index - 1] : 0;
            return maxCursors[_index] - _last;
        }
        public void Insert(int _index, T _value)
        {
            Insert(_index, _value, GetNewElementSize());
        }
        public void Insert(int _index, T _value, float _size)
        {
            association.Insert(_index, _value);
            if (!CanAddNewCursor()) return;
            Squeeze(_size, _index);
            float _lastValue = _index - 1 < 0 ? 0 : maxCursors[_index - 1];
            if (_index > CursorCount) _index = CursorCount;
            maxCursors.Insert(_index, _lastValue + _size);
        }

        private bool CanAddNewCursor() => Count > 1;

        public void RemoveAt(int _index)
        {
            if (_index > 0) maxCursors.RemoveAt(_index - 1);
            association.RemoveAt(_index);
        }

        public void Add(T _item, float _size)
        {
            association.Add(_item);
            if (CanAddNewCursor())
            {
                Squeeze(_size, CursorCount);
                maxCursors.Add(MAX - _size);
            }
        }
        public bool Remove(T _item)
        {
            maxCursors.Remove(association.IndexOf(_item) - 1);
            return association.Remove(_item);
        }

        public void Clear()
        {
            association.Clear();
            maxCursors.Clear();
        }
        public void Add(T _item) => Add(_item, GetNewElementSize());
        public int IndexOf(T _item) => association.IndexOf(_item);
        public bool Contains(T _item) => association.Contains(_item);
        public void CopyTo(T[] _array, int _arrayIndex) => association.CopyTo(_array, _arrayIndex);

        private float GetNewElementSize() => GetEvenStep(Count + 1);
        private void Squeeze(float _size, int _index)
        {
            float _sizeAvailable = MAX - _size;
            for (int _i = 0; _i < CursorCount; _i++)
            {
                maxCursors[_i] = _sizeAvailable * maxCursors[_i];
            }
            for (int _i = _index; _i < CursorCount; _i++)
            {
                maxCursors[_i] += _size;
            }
        }
        #endregion Core

        #region IEnumerator
        public IEnumerator<T> GetEnumerator()
        {
            return association.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return association.GetEnumerator();
        }
        #endregion



    }
#if UNITY_EDITOR
    [System.Flags]
    public enum DistributionEditorViewMode
    {
        none = 0,
        ValuesControl = 1,
        ColorsControl = 2,
    }
#endif
}