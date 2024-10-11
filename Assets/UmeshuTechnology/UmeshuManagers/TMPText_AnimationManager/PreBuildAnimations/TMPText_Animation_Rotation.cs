using UnityEngine;

namespace Umeshu.USystem.TextAnimation
{
    public sealed class TMPText_Animation_Rotation : TMPText_Animation
    {
        protected override AnimationType Animation => AnimationType.Rotation;

        protected override void ApplyOnData(TMPText_AnimationData _animationData)
        {
            float _speed = TryGetParameterValue_Float(PARAMETER_SPEED, 360f);
            _animationData.rotation += Mathf.Repeat(_animationData.time * _speed, 360);
        }
    }
}