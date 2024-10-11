using UnityEngine;

namespace Umeshu.Utility
{
    [RequireComponent(typeof(RectTransform))]
    public class ScaleToMobileSafeArea : CallMethodOnStartEnableAndResolutionChange
    {
        protected override bool CallInAwake => false;
#if PLATFORM_ANDROID || PLATFORM_IOS
        private RectTransform rectTransform;

        protected override void Method() => ScaleToSafeArea();

        private void ScaleToSafeArea()
        {

            rectTransform ??= GetComponent<RectTransform>();

            Rect _safeArea = Screen.safeArea;
            Vector2 _anchorMin = _safeArea.position;
            Vector2 _anchorMax = _safeArea.position + _safeArea.size;
            Vector2 _stockedSizeDelta = rectTransform.sizeDelta;

            _anchorMin.x /= Screen.width;
            _anchorMin.y /= Screen.height;
            _anchorMax.x /= Screen.width;
            _anchorMax.y /= Screen.height;

            rectTransform.anchorMin = _anchorMin;
            rectTransform.anchorMax = _anchorMax;

            rectTransform.sizeDelta = _stockedSizeDelta;

            rectTransform.anchoredPosition = Vector2.zero;

        }
#else
        protected override void Method(){}
#endif
    }
}
