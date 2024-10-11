using System;
using Umeshu.Uf;
using UnityEngine;
using UnityEngine.LowLevel;

namespace Umeshu.Utility
{
    public abstract class CustomUpdatedClass
    {
        [RuntimeInitializeOnLoadMethod]
        private static void AutoCreateInstances()
        {
            Type[] _subtypes = UfReflection.GetSubTypes(typeof(SingletonUpdatedSystem<>), _allAssemblies: true, _checkRawGenerics: true);

            foreach (Type _subtype in _subtypes)
            {
                if (_subtype.IsAbstract) continue;
                if (_subtype.GetCustomAttributes(typeof(AutoCreateInstance), true).Length == 0) continue;

                System.Reflection.ConstructorInfo _constructor = _subtype.GetConstructor(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance, null, new Type[0], null);
                _constructor?.Invoke(new object[0]);
            }
        }

        private void SuscribeToUpdate(PlayerLoopSystem.UpdateFunction _action)
        {
            PlayerLoopSystem _currentSystem = PlayerLoop.GetCurrentPlayerLoop();
            PlayerLoopSystem _mySystem = new() { updateDelegate = _action, type = GetType() };
            int _indexOfUpdateSystem = _currentSystem.subSystemList.IndexOf(Array.Find(_currentSystem.subSystemList, _system => _system.ToString() == "Update")) + 1;
            PlayerLoopSystem[] _subSystemList = _currentSystem.subSystemList.GetArrayWithInserted(_indexOfUpdateSystem, _mySystem);
            PlayerLoopSystem _systemRoot = new() { subSystemList = _subSystemList };
            PlayerLoop.SetPlayerLoop(_systemRoot);
        }

        public CustomUpdatedClass() => SuscribeToUpdate(UpdateMethodWithCheck);
        private void UpdateMethodWithCheck()
        {
            if (Application.isPlaying) UpdateMethod();
        }
        protected abstract void UpdateMethod();

        ~CustomUpdatedClass()
        {
            PlayerLoopSystem _defaultSystems = PlayerLoop.GetDefaultPlayerLoop();
            PlayerLoop.SetPlayerLoop(_defaultSystems);
        }

        public class AutoCreateInstance : Attribute { }
    }

    public abstract class SingletonUpdatedSystem<T> : CustomUpdatedClass where T : SingletonUpdatedSystem<T>, new()
    {
        private static T instance;
        public static T Instance => instance ??= new T();
        protected SingletonUpdatedSystem() { instance = this as T; }
    }
}