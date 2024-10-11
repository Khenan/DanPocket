using UnityEngine;

namespace Umeshu.USystem.TextAnimation
{
    public sealed class TMPText_Animation_Blink : TMPText_Animation
    {
        protected override AnimationType Animation => AnimationType.Blink;

        protected override void ApplyOnData(TMPText_AnimationData _animationData)
        {
            float _frequency = TryGetParameterValue_Float(PARAMETER_FREQUENCY, 1f);
            float _percentageVisible = TryGetParameterValue_Float(PARAMETER_PERCENTAGE_VISIBLE, 0.5f);
            byte _newAlpha = (byte)(Mathf.Repeat(_animationData.time * _frequency, 1f) < _percentageVisible ? 255 : 0);
            _animationData.color = _animationData.color.With(_a: _newAlpha);
        }
    }
}