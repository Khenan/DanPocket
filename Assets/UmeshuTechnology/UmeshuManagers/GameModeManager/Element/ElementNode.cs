using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Umeshu.Uf;
using Umeshu.USystem.Time;
using Umeshu.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Umeshu.USystem
{
    [System.Serializable]
    public class ElementNode : IReadonlyNode<IGameElement>, IGameElement
    {
        private Node<IGameElement> node = new();

        public ElementNode() : base()
        {
        }

        public ElementNode(IGameElement _gameElement)
        {
            node.Value = _gameElement;
            name = _gameElement.Name;
        }

        public void UpdateNodeValue() => node.Value.UpdateNodeValue();

        public void SetNodeValue(IGameElement _value)
        {
            node.Value = _value;
            node.Parent = _value.Hierarchy.node.Parent;
        }

        public virtual IReadonlyNode<IGameElement> this[int _index] => node[_index];
        public IReadonlyNode<IGameElement> Parent => node.Parent;
        public void SetNodeParent(IGameElement _parent) => node.Parent = _parent != null ? _parent.Hierarchy.node : null;
        public IGameElement Value => node.Value;
        public virtual IEnumerable<IReadonlyNode<IGameElement>> Childrens => node;
        public int Count => node.Count;
        public TimeThread Thread => Value.Thread;
        protected virtual IEnumerable<IGameElement> ObjectsToInstantiate => Enumerable.Empty<IGameElement>();
        public ElementNode Hierarchy => this;

        public string Name => name;
        private string name;

        public IEnumerator<IReadonlyNode<IGameElement>> GetEnumerator() => node.GetEnumerator();
        public IEnumerable<IGameElement> GetSubElements() => Childrens.Select(_s => _s.Value);

        public ElementNode GetRoot()
        {
            ElementNode parent = this;
            while (parent.Parent != null)
            {
                parent = parent.Parent.Value.Hierarchy;
            }

            return parent;
        }

        public void InitBranch()
        {
            foreach (UnityEngine.Object _instantiable in ObjectsToInstantiate)
            {
                if (_instantiable == node.Value as UnityEngine.Object) continue;
                IGameElement _instantiated = UnityEngine.Object.Instantiate(_instantiable) as IGameElement;
                if (_instantiated is GameElement _gameElement)
                {
                    if (GetRoot().Value is GameModeSceneProxy sceneProxy)
                    {
                        _gameElement.transform.SetParent(sceneProxy.transform);
                    }
                    else if (this.node.Value is GameElement _parentGameElement)
                    {
                        _gameElement.transform.SetParent(_parentGameElement.transform);
                    }
                }

                AddNewlyCreatedChildren(_instantiated);
            }

            ActOverSubSystem(_subNode => _subNode.InitBranch());
        }

        public void InitScriptData(bool _allowDoubleInitInSameFrame) => ActOverSubSystem(_subNode => _subNode.InitScriptData(_allowDoubleInitInSameFrame));

        public void RemoveAllChildrens()
        {
            foreach (IGameElement _child in GetSubElements().ToArray())
                _child.SetGameElementParent(null, true);
        }

        internal virtual void AddNewlyCreatedChildren(IGameElement _subNode)
        {
            _subNode.SetGameElementParent(node.Value, false);
        }

        public void AddAssetDepedencies(LoadDepedencies _depedencies) => ActOverSubSystem(_subNode => _subNode.AddAssetDepedencies(_depedencies));
        public void LocalReset() => ActOverSubSystem(_subNode => _subNode.LocalReset());
        public virtual void Pause() => ActOverSubSystem(_subNode => _subNode.Pause());
        public virtual void Play() => ActOverSubSystem(_subNode => _subNode.Play());
        public virtual void Resume() => ActOverSubSystem(_subNode => _subNode.Resume());
        public virtual void Stop() => ActOverSubSystem(_subNode => _subNode.Stop());
        public void ActOverSubSystem(Action<IGameElement> _action) => IGameElement.ActOverSubSystem(node.Value, _element => { _action?.Invoke(_element); });

        public void PropagateOverSubSystem(Action<IGameElement> _action) =>
            IGameElement.ActOverSubSystem(node.Value, _element =>
            {
                _action?.Invoke(_element);
                IGameElement.ActOverSubSystem(_element, _action);
            });

        IEnumerator IEnumerable.GetEnumerator() => node.GetEnumerator();
    }
}