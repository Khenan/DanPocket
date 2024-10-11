using Umeshu.Uf;
using Umeshu.Utility;
using UnityEngine;

namespace Umeshu.USystem
{
    public abstract class BaseSystem<T> : GameElement, IGameSystem where T : BaseSystem<T>
    {
        [Header("System")]
        public bool keepParent = false;

        public virtual void OnEnterGameMode(GameModeKey _gameMode) => IGameElement.ActOverSubSystem(this, _sub => (_sub as IGameSystem)?.OnEnterGameMode(_gameMode));

        private void CreateSystemInstance()
        {
            if (IsInstanceExisting())
            {
                Debug.LogError("WARNING : A SYSTEM HAS BEEN INITIALIZED TWICE VIA AWAKE - " + typeof(T).Name);
                Destroy(gameObject);
                return;
            }
            SetInstance();
            if (!keepParent) transform.SetParent(null);
            if (IsInstanceImmortal()) DontDestroyOnLoad(gameObject);
            UmeshuGameManager.Instance.onGameModeLoaded += OnEnterGameMode;
        }

        protected sealed override void GameElementFirstInitialize() => SystemFirstInitialize();
        protected sealed override void GameElementEnableAndReset() => SystemEnableAndReset();
        protected sealed override void GameElementPlay() => SystemPlay();
        protected sealed override void GameElementUpdate() => SystemUpdate();
        public sealed override void Play() { base.Play(); }
        protected override bool Pausable => false;

        protected sealed override void Update()
        {
            base.Update();
            UnitySystemUpdate();
        }
        protected virtual void UnitySystemUpdate() { }

        #region Forced Methods
        protected abstract void SystemFirstInitialize();
        protected abstract void SystemEnableAndReset();
        protected abstract void SystemPlay();
        protected abstract void SystemUpdate();
        #endregion

        public sealed override void InitBranch() { base.InitBranch(); }

        protected abstract bool IsInstanceExisting();
        protected abstract void SetInstance();
        protected abstract bool IsInstanceImmortal();
        protected virtual void AutoLaunch() { }

        #region Security
        protected sealed override void Awake() { base.Awake(); CreateSystemInstance(); AutoLaunch(); } // Prevent usage in childrens
        protected sealed override void Start() { base.Start(); } // Prevent usage in childrens
        protected sealed override void Reset() { base.Reset(); } // Prevent usage in childrens
        #endregion
    }
}