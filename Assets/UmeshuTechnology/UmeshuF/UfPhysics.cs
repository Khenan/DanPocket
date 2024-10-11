using System.Collections.Generic;
using UnityEngine;

namespace Umeshu.Uf
{
    public static class UfPhysics
    {

        public static IEnumerable<Vector2> GetPositionsInsideOfCollider(Collider2D _collider2D, float _sizeIncrement = 1f)
        {
            int _numColumns = Mathf.FloorToInt(_collider2D.bounds.size.x / _sizeIncrement);
            int _numRows = Mathf.FloorToInt(_collider2D.bounds.size.y / _sizeIncrement);
            _numColumns = Mathf.Max(1, _numColumns);
            _numRows = Mathf.Max(1, _numRows);

            Vector2 _start = _collider2D.bounds.min;

            for (int _x = 0; _x < _numColumns; _x++)
            {
                for (int _y = 0; _y < _numRows; _y++)
                {
                    Vector2 _pos = _start + new Vector2(_x, _y);
                    if (_collider2D.OverlapPoint(_pos)) yield return _pos;
                }
            }
        }


    }
}
