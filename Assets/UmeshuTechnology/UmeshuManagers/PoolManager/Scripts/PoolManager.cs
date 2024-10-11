using System;
using System.Collections.Generic;
using Umeshu.Common;
using Umeshu.Uf;
using Umeshu.USystem.Addressable;
using UnityEngine;
namespace Umeshu.USystem.Pool
{
    internal sealed class PoolManager : GameSystem<PoolManager>
    {

        #region Game Element Methods

        protected override void SystemFirstInitialize()
        {
            InitPools();
        }
        protected override void SystemEnableAndReset() { }
        protected override void SystemPlay() { }
        protected override void SystemUpdate() { }

        #endregion

        #region Vars

        private static Dictionary<string, ObjectPool<PoolableGameElement>> pools = new();
        private static Dictionary<string, string> precalculatedKeys = new();
        private static Transform poolParent = null;

        #endregion

        #region Init


        /// <summary>
        /// Create the pools and the parent used for objects
        /// </summary>
        private void InitPools()
        {
            poolParent = new GameObject("Pool Parent").transform;
            poolParent.transform.SetParent(transform);

            pools.Clear();
            precalculatedKeys.Clear();
            AddressableManager.Instance.onLoadFinished += AddPoolsOfObject;
            //UmeshuGameManager.Instance.onGameModeChange += (_s) => CleanAllPools();
        }

        private void AddPoolsOfObject(UPoolableAsset<PoolableGameElement>[] _pools)
        {
            foreach (UPoolableAsset<PoolableGameElement> _poolable in _pools)
                PoolManager.AddPoolOfObject(_poolable.Value, 0);
        }

        internal static void AddPoolOfObject<T>(T _prefab, int _baseInstanceCount) where T : PoolableGameElement
        {
            int _poolIndex = _prefab.GetPoolIndex();
            string _key = GetKeyFromStartPrefab(_prefab, _poolIndex);
            if (!pools.ContainsKey(_key))
            {
                string _keyWithoutPoolIndex = GetKeyWithoutPoolIndex(_key);
                if (!precalculatedKeys.ContainsKey(_keyWithoutPoolIndex)) precalculatedKeys.Add(_keyWithoutPoolIndex, _key);
                pools.Add(_key, new ObjectPool<PoolableGameElement>(Instantiate, _prefab, _baseInstanceCount));
            }
            else pools[_key].SetReferenceObjectIfNull(_prefab);
        }

        #endregion

        #region Put object back in pools

        /// <summary>
        /// Make all object go back to pool
        /// </summary>
        internal static void CleanAllPools()
        {
            foreach (ObjectPool<PoolableGameElement> _value in pools.Values)
            {
                _value.CleanPool();
            }
        }


        /// <summary>
        /// Put an object back in its pool
        /// </summary>
        public static void GoBackToPool(PoolableGameElement _obj)
        {
            ResetObj(_obj, false, Instance);
            if (!_obj.IsAvailable()) Debug.LogError("Poolable Object - Type : " + _obj.GetType().Name + " makeAvailable methods doesn't work");
        }

        #endregion

        internal static void ResetObj(PoolableGameElement _obj, bool _forUse, IGameElement _parent)
        {
            _obj.gameObject.SetActive(_forUse);
            _obj.transform.SetParent(poolParent);
            _obj.SetGameElementParent(_parent);
            if (_obj.MethodTransferer != null) _obj.MethodTransferer.ResetVars();
            if (_forUse) _obj.InitObject(false);
            _obj.LocalReset();
        }

        internal static T GetObject<T>(T _prefabPoolRef, IGameElement _parent) where T : PoolableGameElement
        {
            if (_prefabPoolRef == null)
            {
                Debug.LogError("Error : Tried to add real time pool of object with an no prefab");
                return default;
            }
            int _poolIndex = _prefabPoolRef.GetPoolIndex();
            string _key = GetKey<T>(_poolIndex);
            if (!pools.ContainsKey(_key)) AddPoolOfObject(_prefabPoolRef, 0);
            return GetObject<T>(_poolIndex, _parent);
        }

        internal static void ReleasePoolWithNoReferences()
        {
            List<string> _keysToRemove = new();
            foreach (KeyValuePair<string, ObjectPool<PoolableGameElement>> _pool in pools)
            {
                if (_pool.Value.IsRefObjectNull())
                    _keysToRemove.Add(_pool.Key);
            }
            foreach (string _key in _keysToRemove)
            {
                $"PoolManager : ReleasePoolWithNoReferences - Removing pool with key : {_key}".Log();
                ObjectPool<PoolableGameElement> _pool = pools[_key];
                foreach (PoolableGameElement _obj in _pool)
                {
                    _obj.FlagAsSafeDestroy();
                    Destroy(_obj.gameObject);
                }
                pools.Remove(_key);
                List<string> _keysToRemoveFromPrecalculated = new();
                foreach (KeyValuePair<string, string> _precalculatedKey in precalculatedKeys)
                {
                    if (_precalculatedKey.Value == _key)
                        _keysToRemoveFromPrecalculated.Add(_precalculatedKey.Key);
                }
                foreach (string _keyToRemove in _keysToRemoveFromPrecalculated)
                    precalculatedKeys.Remove(_keyToRemove);
            }
            System.GC.Collect();
        }


        internal static string GetKey<T>(int _poolIndex) where T : PoolableGameElement => typeof(T).Name + "_" + _poolIndex;
        internal static string GetKeyFromStartPrefab<T>(T _prefab, int _poolIndex) where T : PoolableGameElement => _prefab.GetType().Name + "_" + _poolIndex;
        private static bool TryGetExistingKeyOfType<T>(out string _key) where T : PoolableGameElement
        {
            string _wantedKey = GetKeyWithoutPoolIndex(GetKey<T>(0));
            if (precalculatedKeys.ContainsKey(_wantedKey))
            {
                _key = precalculatedKeys[_wantedKey];
                return true;
            }
            _key = "";
            return false;
        }
        private static string GetKeyWithoutPoolIndex(string _key) => _key.Replace('_' + _key.Split('_')[^1], "");
        internal static T GetObject<T>(IGameElement _parent) where T : PoolableGameElement => TryGetExistingKeyOfType<T>(out string _key) ? GetObjectWithKey<T>(_key, _parent) : default;
        internal static T GetObject<T>(int _poolIndex, IGameElement _parent) where T : PoolableGameElement => GetObjectWithKey<T>(GetKey<T>(_poolIndex), _parent);
        internal static T GetObjectWithKey<T>(string _key, IGameElement _parent) where T : PoolableGameElement
        {
            return pools.TryGetValue(_key, out ObjectPool<PoolableGameElement> _value) ? GetObjectAs<T>(_value.GetObject(_parent)) : default;
        }

        internal static T GetObjectAs<T>(PoolableGameElement _obj) where T : PoolableGameElement
        {
            try
            {
                _obj.transform.SetParent(poolParent);
                return (T)Convert.ChangeType(_obj, typeof(T));
            }
            catch (InvalidCastException)
            {
                return default;
            }
        }

        internal static ObjectPool<PoolableGameElement> GetPoolOfObject<T>(int _poolIndex) where T : PoolableGameElement => pools.TryGetValue(GetKey<T>(_poolIndex), out ObjectPool<PoolableGameElement> _value) ? _value : null;

        internal static void DoMethodAction<T>(Action<PoolableGameElement> _action, int _poolIndex) where T : PoolableGameElement
        {
            ObjectPool<PoolableGameElement> _pool = GetPoolOfObject<T>(_poolIndex);

            if (_pool == null)
                return;

            foreach (PoolableGameElement _item in _pool)
                if (_item != null)
                    _action?.Invoke(_item);
        }

        internal static void ResetPool<T>(int _poolIndex) where T : PoolableGameElement => DoMethodAction<T>(GoBackToPool, _poolIndex);

    }

    internal static class PoolManagerExtension
    {
        internal static T GetObject<T>(this IGameElement _parent, T _prefabPoolRef) where T : PoolableGameElement => PoolManager.GetObject(_prefabPoolRef, _parent);
        internal static T GetObject<T>(this IGameElement _parent) where T : PoolableGameElement => PoolManager.GetObject<T>(_parent);
        internal static T GetObject<T>(this IGameElement _parent, int _poolIndex) where T : PoolableGameElement => PoolManager.GetObject<T>(_poolIndex, _parent);
        internal static T GetObjectWithKey<T>(this IGameElement _parent, string _key) where T : PoolableGameElement => PoolManager.GetObjectWithKey<T>(_key, _parent);
    }
}