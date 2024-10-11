using System;
using System.Collections;
using Umeshu.USystem.Time;
using UnityEngine;

namespace Umeshu.Utility
{
    public class AnimD
    {
        protected float duration = 1;
        protected float percentage = 0;
        protected bool finished = false;

        public AnimD(float _duration)
        {
            SetDuration(_duration);
        }

        public void SetPercentage(float _percentage) => this.percentage = _percentage;
        public void SetDuration(float _duration) => this.duration = _duration;
        public float GetPercentage() => this.percentage;
        public bool IsFinished() => finished;
        public float GetDuration() => this.duration;

        public bool FuncTick(Action<float> _updateFunc = null, Action _endFunc = null)
        {
            bool _finishedTick = Tick();
            _updateFunc?.Invoke(GetPercentage());
            if (_finishedTick)
            {
                _endFunc?.Invoke();
                finished = true;
            }
            return _finishedTick;
        }

        public bool Tick() => Tick(GetDuration());

        public bool Tick(float _duration)
        {
            float _lastPercentage = GetPercentage();
            SetPercentage(Mathf.MoveTowards(_lastPercentage, 1, Time.deltaTime / _duration));
            return _lastPercentage < 1 && GetPercentage() == 1;
        }

        public IEnumerator DoAnim(Action<float> _updateFunc, Action _endFunc) => AnimD.DoAnim(this, _updateFunc, _endFunc);

        public static IEnumerator DoAnim(Action<float> _updateFunc, Action _endFunc, float _duration)
        {
            AnimD _animD = new(_duration);
            yield return DoAnim(_animD, _updateFunc, _endFunc);
        }

        public static IEnumerator DoAnim(AnimD _animD, Action<float> _updateFunc, Action _endFunc)
        {
            while (!_animD.FuncTick(_updateFunc, _endFunc))
            {
                yield return TimeManager.WaitForEndOfFrame();
            }
        }
    }
}