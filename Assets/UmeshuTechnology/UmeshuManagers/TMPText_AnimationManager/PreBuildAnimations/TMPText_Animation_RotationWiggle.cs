using System;
using UnityEngine;

namespace Umeshu.USystem.TextAnimation
{
    /// <summary>
    /// <see cref="TMPText_Animation"/> assign to <see cref="TMPText_Animation.AnimationType.RotationWiggle"/> that applies a rotation wiggle effect to the text.
    /// </summary>
    public sealed class TMPText_Animation_RotationWiggle : TMPText_Animation
    {
        protected override AnimationType Animation => AnimationType.RotationWiggle;

        protected override void ApplyOnData(TMPText_AnimationData _animationData)
        {
            float _amplitude = TryGetParameterValue_Float(PARAMETER_AMPLITUDE, 0.1f);
            float _frequency = TryGetParameterValue_Float(PARAMETER_FREQUENCY, 1f);
            float _characterOffset = TryGetParameterValue_Float(PARAMETER_CHARACTER_OFFSET, 0f);
            _animationData.rotation += _amplitude * Mathf.Sin(_animationData.time * _frequency + _characterOffset * _animationData.characterIndex);

        }
    }
}