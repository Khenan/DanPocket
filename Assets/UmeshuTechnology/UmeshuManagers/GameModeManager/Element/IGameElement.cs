using System;
using System.Collections.Generic;
using System.Linq;
using Umeshu.Common;
using Umeshu.Uf;
using Umeshu.USystem.Pool;
using Umeshu.USystem.Time;
using UnityEngine;

namespace Umeshu.USystem
{
    public interface IGameElement
    {
        /// <summary>
        /// Gets the sub-elements of the game element.
        /// </summary>
        /// <returns>An enumerable collection of sub-elements.</returns>
        IEnumerable<IGameElement> GetSubElements();

        /// <summary>
        /// Gets the time thread associated with the game element.
        /// </summary>
        TimeThread Thread { get; }
        public string Name { get; }

        void InitBranch();

        void InitScriptData(bool _allowDoubleInitInSameFrame);

        void LocalReset();

        void Stop();

        void Play();

        void Resume();

        void Pause();

        void UpdateNodeValue();

        void AddAssetDepedencies(LoadDepedencies _depedencies);

        /// <summary>
        /// Gets the hierarchy node of the game element.
        /// </summary>
        ElementNode Hierarchy { get; }

        /// <summary>
        /// Performs the specified action on each sub-system of the game element.
        /// </summary>
        /// <param name="_system">The game element.</param>
        /// <param name="_action">The action to perform.</param>
        static void ActOverSubSystem(IGameElement _system, Action<IGameElement> _action)
        {
            if (_action == null || _system == null) return;
            IEnumerable<IGameElement> _subElements = _system.GetSubElements();
            if (_subElements == null) return;
            foreach (IGameElement _subElement in _subElements.ToArray()) _action.Invoke(_subElement);
        }
    }

    /// <summary>
    /// Provides extension methods for the IGameElement interface.
    /// </summary>
    public static class GameElementExtensions
    {
        internal static void SetGameElementParent(this IGameElement _element, IGameElement _parent, bool _orphanIsWanted = false)
        {
            _element.Hierarchy.SetNodeParent(_parent);
            if (_element.Hierarchy.Parent == null && !_orphanIsWanted)
                $"Please do not create {"ORPHAN".Bold().Color(Color.red).Size(15)} game elements. Its {"BAD!".Bold().Color(Color.red).Size(15)}... Referencing to {_element}".LogError();
        }

        internal static T GetComponentInGameElementChilds<T>(this IGameElement _element)
        {
            foreach (IGameElement _child in _element.GetSubElements())
                if (_child is T _component)
                    return _component;
            return default;
        }

        internal static T[] GetComponentsInGameElementChilds<T>(this IGameElement _element)
        {
            List<T> _components = new();
            foreach (IGameElement _child in _element.GetSubElements())
                if (_child is T _component)
                    _components.Add(_component);
            return _components.ToArray();
        }


        public static T GetChildFromHierarchy<T>(this IGameElement _gameElement, bool _logIfNone)
        {
            T _child = _gameElement.GetComponentInGameElementChilds<T>();
            if (_logIfNone && _child == null) $"No child of type {typeof(T).Name} found in {_gameElement.Name}".LogError("HierarchyError");
            return _child;
        }

        public static T[] GetChildsFromHierarchy<T>(this IGameElement _gameElement, bool _logIfNone)
        {
            T[] _childs = _gameElement.GetComponentsInGameElementChilds<T>();
            if (_logIfNone && _childs.Length == 0) $"No child of type {typeof(T).Name} found in {_gameElement.Name}".LogError("HierarchyError");
            return _childs;
        }

        public static T GetChildFromPool<T>(this IGameElement _gameElement, bool _autoPlayElement, UPoolableAsset<T> _uPoolableAsset = null) where T : PoolableGameElement
        {
            T _child = _uPoolableAsset != null ? _uPoolableAsset.GetAssetFromPool(_gameElement) : PoolManager.GetObject<T>(_gameElement);
            TreatChildFromPool(_gameElement, _child, _autoPlayElement);
            return _child;
        }

        public static T GetChildFromPoolAndReference<T>(this IGameElement _gameElement, bool _autoPlayElement, T _prefabPoolRef) where T : PoolableGameElement
        {
            T _child = PoolManager.GetObject(_prefabPoolRef, _gameElement);
            TreatChildFromPool(_gameElement, _child, _autoPlayElement);
            return _child;
        }

        private static void TreatChildFromPool<T>(this IGameElement _parent, T _child, bool _autoPlayElement) where T : PoolableGameElement
        {
            if (_child == null) $"No child of type {typeof(T).Name} found in {_parent.Name}".LogError("PoolError");
            if (_autoPlayElement) _child.Play();
        }
    }
}