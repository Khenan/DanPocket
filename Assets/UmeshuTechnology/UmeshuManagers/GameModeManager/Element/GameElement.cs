using System;
using System.Collections.Generic;
using Umeshu.Common;
using Umeshu.Uf;
using Umeshu.USystem.Pool;
using Umeshu.USystem.Time;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Umeshu.USystem
{
    public abstract class GameElement : MonoBehaviour, IGameElement
    {
        [Header("Game Element")]
        #region Hierarchy
        [SerializeReference, GenericField(_showAsList: true)]
        public ElementNode hierarchy;
        public ElementNode Hierarchy => hierarchy ??= new(this);
        public IEnumerable<IGameElement> GetSubElements() => Hierarchy.GetSubElements();
        [SerializeField] private bool lookForChilds = false;
        #endregion

        #region Time / Pause
        public TimeThread Thread => paused ? TimeThread.Paused : GetThread();
        public TimeManager_Thread CThread => Thread;
        protected virtual TimeThread GetThread() => (TimeThread)1;
        private bool playing = false;

        public Action onDestroy;
        public virtual bool Paused
        {
            get => paused;
            set
            {
                paused = value && Pausable;
                this.enabled = !paused;
            }
        }
        protected abstract bool Pausable { get; }
        private bool paused = false;
        protected bool CanUpdate => !paused && playing;

        #endregion

        protected virtual void Awake() => UpdateNodeValue();
        protected virtual void Start() { }
        protected virtual void Reset() { }
        protected virtual void Update()
        {
            if (!CanUpdate) return;
            GameElementUpdate();
        }

        #region Forced Methods

        protected abstract void GameElementFirstInitialize();
        protected abstract void GameElementEnableAndReset();
        protected abstract void GameElementPlay();
        protected abstract void GameElementUpdate();

        #endregion

        public static bool HeritsDirectlyFromGameElement(GameElement _gameElement, out string _errorMessage)
        {
            _errorMessage = $"{_gameElement} should not be herited directly, rather use HeritableGameElement";
            return _gameElement is not IGameSystem && _gameElement is not PoolableGameElement && _gameElement is not HeritableGameElement;
        }

        #region Commons Virtual Methods

        private bool hasBeenInitialized = false;
        private int lastInitScriptFrame = -1;

        public void InitObject(bool _allowDoubleInitInSameFrame)
        {
            InitBranch();
            InitScriptData(_allowDoubleInitInSameFrame);
        }

        public virtual void InitBranch()
        {
            if (HeritsDirectlyFromGameElement(this, out string _errorMessage)) _errorMessage.LogError();

            $"InitBranch {this}".Log(Color.green, "GameElementInitialize");


            if (lookForChilds)
            {
                foreach (GameElement _newChild in this.GetComponentsOnObjectAndChildrens<GameElement>())
                {
                    if (_newChild.GetInstanceID() == this.GetInstanceID()) continue;
                    if (_newChild.gameObject == gameObject && _newChild.lookForChilds)
                    {
                        $"Skipping child with same gameObject because they are both trying to get each other as childs : {_newChild.GetHierarchyPath()}".Log();
                        continue;
                    }
                    _newChild.SetGameElementParent(this);
                }
            }
            Hierarchy.InitBranch();
        }

        public void InitScriptData(bool _allowDoubleInitInSameFrame)
        {
            if (lastInitScriptFrame == UnityEngine.Time.frameCount && !_allowDoubleInitInSameFrame) return;
            lastInitScriptFrame = UnityEngine.Time.frameCount;
            _allowDoubleInitInSameFrame = false;

            if (!hasBeenInitialized)
            {
                hasBeenInitialized = true;
                GameElementFirstInitialize();
            }
            LocalReset();
            GameElementEnableAndReset();
            Hierarchy.InitScriptData(_allowDoubleInitInSameFrame);
        }



        public virtual void LocalReset()
        {
            Hierarchy.LocalReset();
            UVarExtension.ResetVars(this);
        }
        public virtual void Stop()
        {
            lastInitScriptFrame = -1;
            playing = false;
            $"Stop {this}".Log(Color.red, "GameElementInitialize");
            Hierarchy.Stop();
        }
        public virtual void Play()
        {
            playing = true;
            Hierarchy.Play();
            Resume();
            GameElementPlay();
        }
        public virtual void Resume()
        {
            Hierarchy.Resume();
            Paused = false;
        }
        public virtual void Pause()
        {
            Hierarchy.Pause();
            Paused = true;
        }

        #endregion

        #region Misc / Interface

        public string Name => name;
        public string NameOfScript => GetType().Name;
        public virtual bool CanReparent => false;
        public virtual void AddAssetDepedencies(LoadDepedencies _depedencies)
        {
            Hierarchy.AddAssetDepedencies(_depedencies);
            List<IUAssetDepedency> _uAssetVariables = this.GetAllVariableOfType<IUAssetDepedency>();

            foreach (IUAssetDepedency _uAssetVariable in _uAssetVariables)
                foreach (AssetLabelReference _assetLabelReference in _uAssetVariable.PackageReferences)
                    _depedencies.depedencies.AddIfNotInside(_assetLabelReference.labelString);

        }
        public virtual bool IsAvailable() => !gameObject.activeInHierarchy;
        public void RemoveAllNodeChildrens() => Hierarchy.RemoveAllChildrens();
        public void UpdateNodeValue() => Hierarchy.SetNodeValue(this);

        #endregion

        protected virtual void OnDestroy() => onDestroy?.Invoke();
    }

    public class LoadDepedencies
    {
        public List<string> depedencies = new();
    }
}
