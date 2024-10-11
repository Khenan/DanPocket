using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Umeshu.Common;
using Umeshu.USystem.LoadingScreen;
using Umeshu.USystem.Scene;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Umeshu.USystem
{
    public abstract partial class UmeshuGameManager : GameSystem<UmeshuGameManager> // GAME MODE MANAGER
    {
        private enum LoadingType
        {
            Single,
            Additive
        }

        [Header("Addressables")] [SerializeField]
        private int nbItemPerLoad = 5;

        internal const string LOG_TRANSITION_CATEGORY = "TransitionLogs";
        internal const string LOG_LOAD_CATEGORY = "LoadingLogs";
        internal const string LOG_LOAD_PACKAGE_CATEGORY = "PackageLoadingLogs";

        public float GetCurrentLoadingProgress() => _gameModeManager.Progress;
        public float GetCurrentFadeInProgress() => _gameModeManager.FadeInProgress;
        public float GetCurrentFadeOutProgress() => _gameModeManager.FadeOutProgress;
        public LoadingState GetCurrentLoadingState() => _gameModeManager.CurrentLoadingState;


        private static GameModeManager GetGameModeManager(LoadingType type) =>
            type switch
            {
                LoadingType.Single => new GameModeManager_SingleLoad(),
                LoadingType.Additive => new GameModeManager_AdditiveLoad(),
                _ => throw new InvalidEnumArgumentException("Invalid LoadingType")
            };

        private abstract class GameModeManager
        {
            protected abstract ILoader[] Loaders { get; }
            public bool Loading { get; private set; }
            public float FadeInProgress { get; private set; } = 0;
            public float FadeOutProgress { get; private set; } = 0;
            public GameModeKey GameMode { get; private set; }
            public GameModeKey LastGameModeKey { get; private set; }
            public LoadingState CurrentLoadingState { get; private set; }
            public float Progress => LoaderExtension.Progress(Loaders);
            private bool FinishedLoading => LoaderExtension.Finished(Loaders);
            
            public UEvent<GameModeKey> onFinishedLoadingMode = new();
            public abstract IEnumerator GoToGameMode(GameModeKey _targetGameMode);
            protected IGameModesInfos GameModesInfos => UmeshuGameManager.Instance.GameModesInfos;
            protected GameModeInfo this[GameModeKey _gameMode] => GameModesInfos[_gameMode];

            protected IEnumerator FadeIn(GameModeKey _target, float _duration)
            {
                FadeInProgress = 0;
                FadeOutProgress = 0;
                this.CurrentLoadingState = LoadingState.FadeIn;
                Instance.onTransitionStart?.Invoke();
                while (FadeInProgress < 1)
                {
                    FadeInProgress = Mathf.MoveTowards(FadeInProgress, 1, UnityEngine.Time.unscaledDeltaTime / _duration);
                    yield return null;
                }

                Loading = true;
                this.CurrentLoadingState = LoadingState.Loading;
                Instance.onChangeGameMode?.Invoke(_target);
            }
            protected IEnumerator WaitForEndOfLoad()
            {
                while (!FinishedLoading) yield return null;
            }
            protected IEnumerator FadeOut(GameModeKey _target, float _duration)
            {
                LastGameModeKey = GameMode;
                this.GameMode = _target;
                Loading = false;
                this.CurrentLoadingState = LoadingState.FadeOut;
                onFinishedLoadingMode?.Invoke(_target);
                while (FadeOutProgress < 1)
                {
                    FadeOutProgress = Mathf.MoveTowards(FadeOutProgress, 1, UnityEngine.Time.unscaledDeltaTime / _duration);
                    yield return null;
                }
                this.CurrentLoadingState = LoadingState.None;
                Instance.onTransitionEnd?.Invoke();
            }

            protected List<string> GetGameModeLoadedScenes(GameModeKey _targetGameMode)
            {
                List<string> _scenesToIgnore = new();
                for (int _i = 0; _i < SceneManager.loadedSceneCount; _i++)
                {
                    string _sceneName = SceneManager.GetSceneAt(_i).name;
                    if (this[_targetGameMode].GetScenes().Contains(_sceneName))
                    {
                        _scenesToIgnore.Add(_sceneName);
                    }
                }

                return _scenesToIgnore;
            }
            
            protected IEnumerable<string> StartOperationOverGameMode(SceneOperator _sceneOperator, GameModeKey _gameMode, IEnumerable<string> _ignore, Action<string> _sceneActionOnScenes)
            {
                if (this[_gameMode] == null || this[_gameMode].GetScenes().Count() == 0) return default;
                IEnumerable<string> _scenes = this[_gameMode].GetScenes().Except(_ignore);
                foreach (string _scene in _scenes) _sceneActionOnScenes?.Invoke(_scene);
                _sceneOperator.StartOperation(_scenes);
                return _scenes;
            }
        }
    }
}