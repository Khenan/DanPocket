using UnityEngine;


namespace Umeshu.USystem.LoadingScreen
{
    public abstract class LoadingUtil : MonoBehaviour
    {
        private void OnEnable()
        {
            LastState = LoadingState.None;
            Init();
        }

        void Update()
        {
            if (LastState != LoadingScreenManager.LoadingState)
            {
                LastState = LoadingScreenManager.LoadingState;
                switch (LoadingScreenManager.LoadingState)
                {
                    case LoadingState.None: FadeOut(1); break;
                    case LoadingState.FadeIn: Init(); break;
                    case LoadingState.Loading: FadeIn(1); break;
                    case LoadingState.FadeOut: Load(1); break;
                }
            }

            switch (LoadingScreenManager.LoadingState)
            {
                case LoadingState.None: break;
                case LoadingState.FadeIn: FadeIn(LoadingScreenManager.FadeInProgress); break;
                case LoadingState.Loading: Load(LoadingScreenManager.LoadingProgress); break;
                case LoadingState.FadeOut: FadeOut(LoadingScreenManager.FadeOutProgress); break;
            }
        }

        public LoadingState LastState { get; private set; } = LoadingState.None;
        public abstract void Init();
        public abstract void FadeIn(float _progress);
        public abstract void Load(float _progress);
        public abstract void FadeOut(float _progress);
    }
}