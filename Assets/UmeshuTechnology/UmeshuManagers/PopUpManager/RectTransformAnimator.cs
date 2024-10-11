using System.Collections.Generic;
using Umeshu.USystem.Time;
using UnityEngine;

namespace Umeshu.USystem.PopUp
{
    public class RectTransformAnimator : MonoBehaviour
    {
        [SerializeField] private TimeThread timeThread = TimeThread.General;
        [SerializeField] public List<RectTransformAnimationData> animationsDatas = new();

        public void UpdateRectTransforms()
        {
            float _deltaTime = TimeManager.GetDeltaTime(timeThread);
            foreach (RectTransformAnimationData _animationData in animationsDatas) _animationData.Update(_deltaTime);
        }

        public void AppearAll()
        {
            foreach (RectTransformAnimationData _transformData in animationsDatas) _transformData.Appear();
        }
        public void DisappearAll()
        {
            foreach (RectTransformAnimationData _transformData in animationsDatas) _transformData.Disappear();
        }

        public RectTransformAnimationData GetRectTransformData(int _index) => animationsDatas[_index];
    }
}