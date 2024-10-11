using System;
using System.Collections.Generic;

namespace Umeshu.USystem
{
    [Serializable]
    public static class GameElementFinder
    {
        public static T GetSystem<T>() where T : class, IGameElement
        {
            return UmeshuGameManager.Instance.GetSystem<T>();
        }
        public static List<T> GetSystems<T>() where T : class, IGameElement
        {
            return UmeshuGameManager.Instance.GetSystems<T>();
        }

        public static T GetSystem<T>(this IGameElement _gameSystem) where T : class, IGameElement
        {
            if (_gameSystem == null) return null;
            if (_gameSystem.IsSystem(out T _value))
            {
                return _value;
            }
            foreach (IGameElement _subSystem in _gameSystem.GetSubElements())
            {
                if (_subSystem.IsSystem(out _value)) return _value;
            }
            return null;
        }

        public static List<T> GetSystems<T>(this IGameElement _gameSystem) where T : IGameElement
        {
            List<T> _values = new();
            AddSystemsToListRecursive(_gameSystem, _values);
            return _values;
        }

        private static void AddSystemsToListRecursive<T>(IGameElement _gameSystem, List<T> _values) where T : IGameElement
        {
            if (_gameSystem == null) return;
            if (_gameSystem.IsSystem(out T _value))
            {
                _values.Add(_value);
            }
            IEnumerable<IGameElement> _subElements = _gameSystem.GetSubElements();
            if (_subElements == null) return;
            foreach (IGameElement _subSystem in _subElements)
            {
                AddSystemsToListRecursive(_subSystem, _values);
            }
            return;
        }

        public static bool IsSystem<T>(this IGameElement _system, out T _value) where T : IGameElement
        {
            if (_system is T _cast)
            {
                _value = _cast;
                return true;
            }
            {
                _value = default;
                return false;
            }
        }
    }
}