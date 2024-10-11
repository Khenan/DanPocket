using UnityEngine;
using UnityEngine.UI;

namespace Umeshu.Utility
{
    [RequireComponent(typeof(CanvasScaler))]
    public class CanvasScalerMatchFromScreenSize : CallMethodOnStartEnableAndResolutionChange
    {
        private CanvasScaler canvasScaler;

        protected override bool CallInAwake => true;
        protected override void Method() => SetRightScale();


        private void SetRightScale()
        {
            canvasScaler ??= GetComponent<CanvasScaler>();
            canvasScaler.matchWidthOrHeight = Screen.width > Screen.height ? 1 : 0;
        }
    }
}
