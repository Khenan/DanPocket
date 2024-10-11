using UnityEngine;

namespace Umeshu.USystem.TextAnimation
{
    public sealed class TMPText_Animation_CharacterAppearance : TMPText_Animation
    {
        protected override AnimationType Animation => AnimationType.CharacterAppearance;

        protected override void ApplyOnData(TMPText_AnimationData _animationData)
        {
            float _time = TryGetParameterValue_Float(PARAMETER_TIME, 0.2f);
            float _characterApparitionOffset = TryGetParameterValue_Float(PARAMETER_CHARACTER_APPARITION_OFFSET, 0f);

            float _percentageApparition = _animationData.GetPercentageOfApparition(_time, _characterApparitionOffset);
            float _startScale = TryGetParameterValue_Float(PARAMETER_START_SCALE, 3f);
            float _startAlpha = TryGetParameterValue_Float(PARAMETER_START_ALPHA, 0f);

            _animationData.scale *= Mathf.Lerp(_startScale, 1, _percentageApparition);
            byte _alpha = (byte)Mathf.Lerp(_startAlpha, 255, _percentageApparition);
            _animationData.color = _animationData.color.With(_a: _alpha);
        }
    }
}