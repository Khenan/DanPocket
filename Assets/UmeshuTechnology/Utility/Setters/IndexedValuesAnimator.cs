using Umeshu.USystem;
using Umeshu.USystem.Time;
using UnityEngine;
using UnityEngine.Serialization;

namespace Umeshu.Utility
{
    public abstract class IndexedValuesAnimator : HeritableGameElement
    {
        const float IDLE_ANIMATION_WRAP = 1;
        /// <summary>
        /// 1 => All values have started their appear. So 1 + appearFadeGap (range 0 - 1) => All values have completed their appear. That's why the max value is set to 2.
        /// </summary>
        const float APPEAR_MAX = 2;

        [Header("Default Value")]
        [SerializeField] private float updateWait = 0f;
        [SerializeField, FormerlySerializedAs("size")] private float defaultValue = 1;
        [SerializeField, FormerlySerializedAs("sizeAnimationCurve")] private OptionalVar<AnimationCurve> defaultMultiplicatorCurve;

        [Header("Appear")]
        [SerializeField, Range(0, 1)] private float appear = 1;
        [SerializeField, Range(0, 1)] private float appearFadeGap = 0.1f;
        [SerializeField] private OptionalVar<AnimationCurve> appearAnimationCurve;

        [Header("Idle Animation")]
        [SerializeField, Tooltip("Use negative numbers to inverse the animation.")] private float idleAnimationDuration = 1;
        [SerializeField] private float idleAnimationApplyOnGap = 0.4f;
        [SerializeField] private OptionalVar<AnimationCurve> idleAnimationCurve;

        private float idleAnimationPos = 1;
        private float updateWaitValue = 0;

        protected abstract float GetMax();
        protected abstract void SetValue(int _index, float _size);

        public void SetAppearValue(float _appearValue)
        {
            appear = Mathf.Clamp(_appearValue, 0, 1);
        }

        protected override void GameElementEnableAndReset()
        {
            updateWaitValue = 0;
        }
        protected override void GameElementFirstInitialize() { }
        protected override void GameElementPlay() { }
        protected override void GameElementUpdate()
        {
            UpdateValues(_forceUpdate: false);
        }

        public virtual void UpdateValues(bool _forceUpdate = false)
        {
            float _deltaTime = TimeManager.GetDeltaTime(TimeThread.Player);

            IdleAnimationUpdate(_deltaTime);

            if (_forceUpdate || GetUpdateNeeded(_deltaTime))
            {
                float _trueAppear = appear * (appear + appearFadeGap);
                float _max = GetMax();

                for (int _i = 0; _i < _max; _i++)
                {
                    float _factor = (float)_i / _max;
                    float _appearMult = (_factor < _trueAppear) ? 1 : 0;

                    float _value = _appearMult;

                    if (_value != 0)
                    {
                        _value = GetValue(_trueAppear, _factor, _value);
                    }

                    SetValue(_i, _value);
                }
            }
        }

        private bool GetUpdateNeeded(float _deltaTime)
        {
            bool _canUpdate = updateWaitValue <= 0;

            if (_canUpdate)
                updateWaitValue = updateWait;
            else
                updateWaitValue -= _deltaTime;

            return _canUpdate;
        }

        private float GetValue(float _trueAppear, float _factor, float _defaulValue)
        {
            _defaulValue *= defaultValue;

            if (defaultMultiplicatorCurve.Enabled)
            {
                _defaulValue *= defaultMultiplicatorCurve.Value.Evaluate(_factor);
            }

            if (appearAnimationCurve.Enabled)
            {
                float _appearFactor = Mathf.InverseLerp(Mathf.Clamp(_trueAppear - appearFadeGap, 0, APPEAR_MAX), _trueAppear, _factor);
                _defaulValue *= appearAnimationCurve.Value.Evaluate(_appearFactor);
            }

            if (idleAnimationCurve.Enabled && idleAnimationApplyOnGap > 0)
            {
                float _ownAnimationFactor = idleAnimationPos + (_factor / idleAnimationApplyOnGap);
                _ownAnimationFactor -= (int)_ownAnimationFactor;

                _defaulValue *= idleAnimationCurve.Value.Evaluate(_ownAnimationFactor);
            }

            return _defaulValue;
        }

        private void IdleAnimationUpdate(float _deltaTime)
        {
            idleAnimationPos -= _deltaTime / idleAnimationDuration;

            if (idleAnimationPos > IDLE_ANIMATION_WRAP)
            {
                idleAnimationPos -= IDLE_ANIMATION_WRAP;
            }
            else if (idleAnimationPos <= 0)
            {
                idleAnimationPos += IDLE_ANIMATION_WRAP;
            }
        }
    }
}