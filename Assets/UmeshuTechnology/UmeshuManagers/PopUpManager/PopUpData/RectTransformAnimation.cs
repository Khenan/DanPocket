using System;
using Umeshu.Utility;
using UnityEngine;

namespace Umeshu.USystem.PopUp
{
    /// <summary>
    /// Used to store an animation that can be assigned to a <see cref="RectTransformAnimationData"/>.
    /// How to add a new parameter to animate?
    /// <list type="number">
    /// <item>Edit <see cref="RectTransformAnimation"/> to add a new optional <see cref="RectTransformAnimationCurve"/>.</item>
    /// <item>Then go to <see cref="RectTransformAnimationData.UpdateRectTransform"/> and edit the code to modify the desired parameter.</item>
    /// </list>
    /// </summary>
    [CreateAssetMenu(fileName = "RectTransformAnimation_", menuName = "ScriptableObjects/Animation/RectTransformAnimation")]
    public class RectTransformAnimation : ScriptableObject
    {
        [Header("Durations")]
        public float appearDuration;
        public float idleDuration;
        public float disappearDuration;
        [Space(16)]
        [Header("Position")]
        public OptionalVar<RectTransformAnimationCurve> curvePositionX;
        public OptionalVar<RectTransformAnimationCurve> curvePositionY;
        [Space(8)]
        [Header("Scale")]
        public OptionalVar<RectTransformAnimationCurve> curveScaleX;
        public OptionalVar<RectTransformAnimationCurve> curveScaleY;
        [Space(8)]
        [Header("Rotation")]
        public OptionalVar<RectTransformAnimationCurve> curveRotation;
    }

    /// <summary>
    /// Link an <see cref="RectTransformAnimation"/> to a <see cref="RectTransform"/> and allow some cutsomization.
    /// </summary>
    [Serializable]
    public class RectTransformAnimationData
    {
        public RectTransform rectTransform;
        public float duration;
        public float appearDelay;
        public RectTransformAnimation animation;

        [HideInInspector] public RectTransformAnimationState currentState;
        [HideInInspector] public float ownTime;

        // Custom Animation
        private string customAnimationKey;
        private float customAnimationDuration;
        private int customAnimationPlayNumber;
        private bool askToPlayCustomAnimation;
        private bool isPlayingCustomAnimation;
        public bool IsPlayingCustomAnimation => isPlayingCustomAnimation;
        private RectTransformAnimationState customAnimationOverides;

        public void SetTime(float _time)
        {
            ownTime = _time;
        }

        public void Appear(bool _resetCustomAnimation = false)
        {
            currentState = RectTransformAnimationState.Appear;
            SetTime(-appearDelay);
            if (_resetCustomAnimation) { ResetCustomAnimation(false); }
        }
        public void Disappear(bool _resetCustomAnimation = false)
        {
            currentState = RectTransformAnimationState.Disappear;
            SetTime(0);
            if (_resetCustomAnimation) { ResetCustomAnimation(false); }
        }

        public void SetCustomAnimation(string _name, float _duration = 1, bool _restartTime = true, int _playNumber = -1, RectTransformAnimationState _overideState = RectTransformAnimationState.Idle)
        {
            askToPlayCustomAnimation = true;
            customAnimationKey = _name;
            customAnimationDuration = _duration;
            customAnimationOverides = _overideState;
            customAnimationPlayNumber = _playNumber;

            if (_restartTime)
            { SetTime(0); }
        }
        public void ResetCustomAnimation(bool _restartTime = true)
        {
            SetCustomAnimation("", 1, _restartTime);
            askToPlayCustomAnimation = false;
        }

        public void Update(float _deltaTime)
        {
            isPlayingCustomAnimation = CheckForCustomAnimationToBePlayed();

            UpdateOwnTime(_deltaTime);

            //Check for switching to idle state
            if (currentState == RectTransformAnimationState.Appear && ownTime > 1)
            {
                currentState = RectTransformAnimationState.Idle;
                currentState = RectTransformAnimationState.Idle;
            }

            UpdateRectTransform();
        }
        private void UpdateOwnTime(float _deltaTime)
        {
            float _timeMult = 1 / duration;

            if (isPlayingCustomAnimation)
            {
                _timeMult *= 1 / customAnimationDuration;
            }
            else if (animation != null)
            {
                switch (currentState)
                {
                    case RectTransformAnimationState.Appear:
                        _timeMult *= 1 / animation.appearDuration;
                        break;
                    case RectTransformAnimationState.Idle:
                        _timeMult *= 1 / animation.idleDuration;
                        break;
                    case RectTransformAnimationState.Disappear:
                        _timeMult *= 1 / animation.disappearDuration;
                        break;
                }
            }

            float _ownTimePreviousFrac = ownTime - (int)ownTime;

            ownTime += (_deltaTime * _timeMult);
            float _ownTime = ownTime;
            if (_ownTime < 0)
            { _ownTime = 0; }

            if (_ownTimePreviousFrac > (ownTime - (int)ownTime))
            {
                if (isPlayingCustomAnimation)
                {
                    customAnimationPlayNumber--;
                    if (customAnimationPlayNumber == 0)
                    {
                        ResetCustomAnimation();
                    }
                }
                OnAnimationEnd();
            }
        }
        private void UpdateRectTransform()
        {
            if (rectTransform == null) return;

            float _xPosition = EvaluateAppearanceDataCurve(animation.curvePositionX, rectTransform.anchoredPosition.x);
            float _yPosition = EvaluateAppearanceDataCurve(animation.curvePositionY, rectTransform.anchoredPosition.y);

            float _xScale = EvaluateAppearanceDataCurve(animation.curveScaleX, rectTransform.localScale.x);
            float _yScale = EvaluateAppearanceDataCurve(animation.curveScaleY, rectTransform.localScale.y);

            float _rotation = EvaluateAppearanceDataCurve(animation.curveRotation, rectTransform.localRotation.z);

            rectTransform.anchoredPosition = new Vector2(_xPosition, _yPosition);
            rectTransform.localScale = new Vector2(_xScale, _yScale);
            rectTransform.localRotation = Quaternion.Euler(0, 0, _rotation);
        }
        private float EvaluateAppearanceDataCurve(OptionalVar<RectTransformAnimationCurve> _curveAppearanceData, float _defaultValue = 0)
        {
            AnimationCurve _currentCurve = null;
            float _time = ownTime;

            if (_curveAppearanceData.Enabled)
            {
                RectTransformAnimationCurve _rectTransformAnimationCurve = _curveAppearanceData.Value;

                if (isPlayingCustomAnimation)
                {
                    if (_rectTransformAnimationCurve.curveAdditionals.ContainsKey(customAnimationKey))
                        _currentCurve = _rectTransformAnimationCurve.curveAdditionals[customAnimationKey];
                    _time -= (int)_time; //Loop the curve
                }
                else if (_rectTransformAnimationCurve.curveAppear.Enabled && (currentState == RectTransformAnimationState.Appear))
                {
                    _currentCurve = _rectTransformAnimationCurve.curveAppear.Value;
                }
                else if (_rectTransformAnimationCurve.curveDisappear.Enabled && (currentState == RectTransformAnimationState.Disappear))
                {
                    _currentCurve = _rectTransformAnimationCurve.curveDisappear.Value;
                }
                else if (_rectTransformAnimationCurve.curveIdle.Enabled)
                {
                    _currentCurve = _rectTransformAnimationCurve.curveIdle.Value;
                    _time -= (int)_time; //Loop the curve
                }
            }

            return (_currentCurve != null) ? _currentCurve.Evaluate(_time) : _defaultValue;
        }

        private bool CheckForCustomAnimationToBePlayed()
        {
            bool _isCustomAnimation = askToPlayCustomAnimation;
            if (_isCustomAnimation) _isCustomAnimation = customAnimationOverides.HasFlag(currentState);
            return _isCustomAnimation;
        }
        private void OnAnimationEnd() { } //Could be cool to add if need a callback when the animation ends
    }

    /// <summary>
    /// A struct to store the animation curves for one parametter of the <see cref="RectTransformAnimation"/>
    /// </summary>
    [Serializable]
    public struct RectTransformAnimationCurve
    {
        public OptionalVar<AnimationCurve> curveAppear;
        public OptionalVar<AnimationCurve> curveIdle;
        public OptionalVar<AnimationCurve> curveDisappear;

        public SerializedDictionary<string, AnimationCurve> curveAdditionals;
    }

    /// <summary>
    /// Define the current state of the animation
    /// </summary>
    public enum RectTransformAnimationState
    {
        Appear,
        Idle,
        Disappear
    }
}