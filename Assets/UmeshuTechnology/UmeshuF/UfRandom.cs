using System.Collections.Generic;
using UnityEngine;

namespace Umeshu.Uf
{
    public static class UfRandom
    {
        #region Perlin Noise
        public static float GetPerlinFloat(float _seed, float _time) => UfMath.ZeroOne_To_MinusOneOne(Mathf.PerlinNoise(_time + _seed, _time + _seed));
        public static Vector3 GetPerlinVector(float _seed, float _time) => new(GetPerlinFloat(_seed, _time), GetPerlinFloat(_seed * 2, _time), GetPerlinFloat(_seed * 3, _time));
        #endregion

        #region Random Inside
        public static T RandomInside<T>(this IList<T> _list) => _list[UnityEngine.Random.Range(0, _list.Count)];
        public static T RandomInside<T>(this T[] _array) => _array[UnityEngine.Random.Range(0, _array.Length)];
        #endregion

        public static float GetRandomWithoutRange(Vector2 _range, float _maxValue = 1)
        {
            float _rangeSize = _range.y - _range.x;
            if (_rangeSize >= _maxValue) return 0;
            float _random = Random.Range(0, _maxValue - _rangeSize);
            return _random > _range.x ? _random + _rangeSize : _random;
        }
    }
}
