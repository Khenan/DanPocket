using System;
using System.Collections;
using Umeshu.Common;
using Umeshu.Uf;
using Umeshu.USystem.Addressable;
using Umeshu.USystem.GameData;
using Umeshu.USystem.LoadingScreen;
using Umeshu.USystem.Pool;
using Umeshu.USystem.Scene;
using Umeshu.USystem.Time;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Umeshu.USystem
{
    public abstract partial class UmeshuGameManager : GameSystem<UmeshuGameManager> // MAIN
    {
        protected abstract IGameModesInfos GameModesInfos { get; }
        public GameModeKey CurrentGameModeKey => _gameModeManager.GameMode;
        public GameModeKey LastGameModeKey => _gameModeManager.LastGameModeKey;
        [SerializeField] private LoadingType _loadingType = LoadingType.Single;
        private GameModeManager _gameModeManager;
        [SerializeField] private ITransitionner transitionner;
        [SerializeField] private LogCategoryData logCategoryData;
        private readonly SceneLoader transitionSceneLoader = new();

        #region Events

        private UEvent onTransitionStart = new();
        private UEvent onTransitionEnd = new();
        private UEvent<GameModeKey> onChangeGameMode = new(), onFinishedLoadingMode = new();

        public event Action onStartTransition
        {
            add { onTransitionStart += value; }
            remove { onTransitionStart -= value; }
        }

        public event Action<GameModeKey> onGameModeChange
        {
            add { onChangeGameMode += value; }
            remove { onChangeGameMode -= value; }
        }

        public event Action<GameModeKey> onGameModeLoaded
        {
            add { onFinishedLoadingMode += value; }
            remove { onFinishedLoadingMode -= value; }
        }

        public event Action onEndTransition
        {
            add { onTransitionEnd += value; }
            remove { onTransitionEnd -= value; }
        }

        #endregion

        public abstract AssetLabelReference[] GetPermanentPackages();

        public string GetTransitionSceneName() => GameModesInfos.TransitionSceneName;

        protected sealed override void AutoLaunch()
        {
            _gameModeManager = GetGameModeManager(_loadingType);
            logCategoryData?.AddCategoriesToLogger();
            GameModesInfos.CreateGameModeKeys();

            InitGameModeSceneProxyRef();
            AddSystem<GameDataManager>();
            AddSystem<TimeManager>();
            AddSystem<AddressableManager>();
            AddSystem<PoolManager>();
            AddSystem<LoadingScreenManager>();
            InitObject(false);
            Play();

            _gameModeManager.onFinishedLoadingMode += OnFinishedLoadingMode;
            StartCoroutine(LoadTransitionSceneThenGoToGameMode());
        }

        private void AddSystem<T>() where T : GameSystem<T>
        {
            T _gameElement = gameObject.AddComponent<T>();
            Hierarchy.AddNewlyCreatedChildren(_gameElement);
        }

        #region System Methods

        protected sealed override void SystemFirstInitialize()
        {
        }

        protected sealed override void SystemEnableAndReset()
        {
        }

        protected sealed override void SystemPlay()
        {
        }

        protected sealed override void SystemUpdate()
        {
        }

        #endregion

#if UNITY_EDITOR

        private void OnApplicationQuit() => AddLogCategoriesToData();

        private void AddLogCategoriesToData()
        {
            if (logCategoryData == null) return;
            int _lastSize = logCategoryData.logCategories.Count;
            foreach (string _category in UfLogger.newCategories)
            {
                if (logCategoryData.logCategories.Find(_c => _c.Value == _category) == null)
                    logCategoryData.logCategories.Add(new(_category, true));
            }

            if (_lastSize != logCategoryData.logCategories.Count)
                UnityEditor.EditorUtility.SetDirty(logCategoryData);
        }
#endif

        private IEnumerator LoadTransitionSceneThenGoToGameMode()
        {
            transitionSceneLoader.StartOperation(new string[] { GetTransitionSceneName() });
            while (!transitionSceneLoader.IsDone)
                yield return null;
            LoadingScreenManager.Instance.FinishedLoadingLoadingScene();
            GoToGameMode(GameModesInfos.StartGameMode);
        }

        public void GoToGameMode(GameModeKey _gameMode)
        {
            if (_gameMode == CurrentGameModeKey)
            {
                Debug.LogError($"Already in {_gameMode}");
                return;
            }

            if (_gameModeManager.Loading)
            {
                Debug.LogError($"Already loading");
                return;
            }

            if (transitionner != null)
            {
                if (transitionner.InTransition)
                {
                    Debug.LogError($"Already in transition");
                    return;
                }

                transitionner.DoTransitionEnter(LoadGameMode);
            }
            else LoadGameMode();

            void LoadGameMode()
            {
                $"Go to game mode {_gameMode}".Log(Color.cyan, "GameModeGoTo");
                GameModeEnd();
                StartCoroutine(_gameModeManager.GoToGameMode(_gameMode));
            }
        }

        protected abstract void GameModeStart();
        protected abstract void GameModeEnd();


        private void OnFinishedLoadingMode(GameModeKey _gameMode)
        {
            GameModeStart();

            if (transitionner != null) transitionner.DoTransitionExit(TriggerEventOnEndTransition);
            else TriggerEventOnEndTransition();

            void TriggerEventOnEndTransition() => onFinishedLoadingMode?.Invoke(_gameMode);
        }
    }

    public interface ITransitionner
    {
        public bool InTransition { get; }
        public void DoTransitionEnter(Action _onFinished);
        public void DoTransitionExit(Action _onFinished);
    }
}