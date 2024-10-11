using UnityEngine;

namespace Umeshu.USystem.TextAnimation
{
    public sealed class TMPText_Animation_WaveX : TMPText_Animation
    {
        protected override AnimationType Animation => AnimationType.WaveX;

        protected override void ApplyOnData(TMPText_AnimationData _animationData)
        {
            float _amplitude = TryGetParameterValue_Float(PARAMETER_AMPLITUDE, 0.1f);
            float _frequency = TryGetParameterValue_Float(PARAMETER_FREQUENCY, 1f);
            float _characterOffset = TryGetParameterValue_Float(PARAMETER_CHARACTER_OFFSET, 0f);
            _animationData.offset += _amplitude * Mathf.Sin(_animationData.time * _frequency + _characterOffset * _animationData.characterIndex) * Vector2.right;
        }
    }
}