using System.Collections.Generic;
using Umeshu.USystem.Time;
using UnityEngine;

namespace Umeshu.USystem
{
    public abstract class ScriptableElement : ScriptableObject, IGameElement
    {
        public TimeThread Thread => GetThread();
        public TimeManager_Thread CThread => Thread;

        public ElementNode Hierarchy => node;

        public bool CanReparent => false;
        public string Name => name;

        [SerializeReference, GenericField]
        private ElementNode node;
        protected virtual TimeThread GetThread() => (TimeThread)1;
        [Header("Scriptable Element")]
        [SerializeReference] private List<ScriptableElement> childElements;

        public IEnumerable<IGameElement> GetSubElements() => childElements;

        public virtual void InitBranch() => IGameElement.ActOverSubSystem(this, _sub => _sub.InitBranch());
        public virtual void InitScriptData(bool _allowDoubleInitInSameFrame) => IGameElement.ActOverSubSystem(this, _sub => _sub.InitScriptData(_allowDoubleInitInSameFrame));

        public virtual void LocalReset() => IGameElement.ActOverSubSystem(this, _sub => _sub.LocalReset());
        public virtual void Stop() => IGameElement.ActOverSubSystem(this, _sub => _sub.Stop());
        public virtual void Play() => IGameElement.ActOverSubSystem(this, _sub => _sub.Play());
        public virtual void Resume() => IGameElement.ActOverSubSystem(this, _sub => _sub.Resume());
        public virtual void Pause() => IGameElement.ActOverSubSystem(this, _sub => _sub.Pause());
        public virtual void AddAssetDepedencies(LoadDepedencies _depedencies) => IGameElement.ActOverSubSystem(this, _sub => _sub.AddAssetDepedencies(_depedencies));

        public void UpdateNodeValue() => node.SetNodeValue(this);
    }
}