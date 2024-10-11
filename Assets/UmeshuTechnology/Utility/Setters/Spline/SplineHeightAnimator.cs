using UnityEngine;
using UnityEngine.U2D;

namespace Umeshu.Utility
{
    public class SplineHeightAnimator : IndexedValuesAnimator
    {
        [SerializeField] private Spline spline;

        protected override float GetMax() => spline.GetPointCount();
        protected override void SetValue(int _index, float _size) { spline.SetHeight(_index, _size); }
    }
}
