using System;
using System.Collections;
using Umeshu.USystem.Time;

namespace Umeshu.Utility
{
    public class AnimD_Specific<T> : AnimD
    {
        public AnimD_Specific(float _duration) : base(_duration)
        {

        }

        private T lastUsedObject;
        private Action<T, float> lastUpdateFunc;
        private Action<T> lastEndFunc;

        public bool FuncTick(T _obj, Action<T, float> _updateFunc = null, Action<T> _endFunc = null)
        {
            lastUsedObject = _obj;
            lastUpdateFunc = _updateFunc;
            lastEndFunc = _endFunc;
            return base.FuncTick(LocalPercentageMethod, LocalEndFunc);
        }

        private void LocalPercentageMethod(float _percentage) => lastUpdateFunc?.Invoke(lastUsedObject, _percentage);

        private void LocalEndFunc() => lastEndFunc?.Invoke(lastUsedObject);



        public IEnumerator DoAnim(T _obj, Action<T, float> _updateFunc, Action<T> _endFunc) => AnimD_Specific<T>.DoAnim(_obj, this, _updateFunc, _endFunc);

        public static IEnumerator DoAnim(T _obj, Action<T, float> _updateFunc, Action<T> _endFunc, float _duration)
        {
            AnimD_Specific<T> _animD = new(_duration);
            yield return DoAnim(_obj, _animD, _updateFunc, _endFunc);
        }

        public static IEnumerator DoAnim(T _obj, AnimD_Specific<T> _animD, Action<T, float> _updateFunc, Action<T> _endFunc)
        {
            while (!_animD.FuncTick(_obj, _updateFunc, _endFunc))
            {
                yield return TimeManager.WaitForEndOfFrame();
            }
        }
    }
}