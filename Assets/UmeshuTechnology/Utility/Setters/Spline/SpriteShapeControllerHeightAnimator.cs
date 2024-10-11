using UnityEngine;
using UnityEngine.U2D;

namespace Umeshu.Utility
{
    public class SpriteShapeControllerHeightAnimator : IndexedValuesAnimator
    {
        [SerializeField] private SpriteShapeController spriteShapeController;

        protected override float GetMax() => spriteShapeController.spline.GetPointCount();
        protected override void SetValue(int _index, float _size) { spriteShapeController.spline.SetHeight(_index, _size); }
    }
}