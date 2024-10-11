using UnityEngine;

namespace Umeshu.Utility
{
    public abstract class CallMethodOnStartEnableAndResolutionChange : MonoBehaviour
    {
        protected abstract bool CallInAwake { get; }
        protected abstract void Method();

        private float lastScreenWidth;
        private float lastScreenHeight;

        protected virtual void Awake() { if (CallInAwake) CallMethod(); }
        protected virtual void Start() => CallMethod();
        protected virtual void OnEnable() => CallMethod();
        protected virtual void Update() { if (lastScreenWidth != Screen.width || lastScreenHeight != Screen.height) CallMethod(); }

        private void CallMethod()
        {
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
            Method();
        }

    }
}