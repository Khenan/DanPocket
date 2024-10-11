using UnityEngine;

namespace Umeshu.Utility
{
    [RequireComponent(typeof(RectTransform))]
    public class ReajustingRatioIfWidthSuperiorToHeight : CallMethodOnStartEnableAndResolutionChange
    {
        [SerializeField] private float wantedRatio = 1;

        private RectTransform rectTransform;
        private RectTransform parentRectTransform;

        protected override bool CallInAwake => false;

        private void ScaleToWantedRatioInsideParent(float _ratio)
        {
            rectTransform ??= GetComponent<RectTransform>();
            parentRectTransform ??= transform.parent.GetComponent<RectTransform>();

            if (Screen.width < Screen.height)
            {
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, parentRectTransform.rect.size.x);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, parentRectTransform.rect.size.y);
                return;
            }

            Vector2 _parentSize = parentRectTransform.rect.size;
            float _targetWidth = _parentSize.y * _ratio;
            float _targetHeight = _parentSize.x / _ratio;

            if (_targetWidth <= _parentSize.x)
            {
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _targetWidth);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _parentSize.y);
            }
            else
            {
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _parentSize.x);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _targetHeight);
            }
        }

        protected override void Method() => ScaleToWantedRatioInsideParent(wantedRatio);
    }
}
