using System;
using System.Collections;
using System.Collections.Generic;
using Umeshu.Uf;
using UnityEngine;

namespace Umeshu.USystem.Pool
{
    internal class ObjectPool<T> : IList<T> where T : PoolableGameElement
    {
        private readonly List<T> objectsInPool = new();
        private readonly Func<T, T> creationMethod = null;
        private T referenceObject = default;

        internal ObjectPool(Func<T, T> _method, T _objRef, int _baseInstanceCount)
        {
            this.creationMethod = _method;
            this.referenceObject = _objRef;
            for (int _i = 0; _i < _baseInstanceCount; _i++)
                TryToCreateObject().gameObject.SetActive(false);
        }

        internal void SetReferenceObjectIfNull(T _obj) => referenceObject ??= _obj;

        internal U GetRefObjectAs<U>() where U : T => referenceObject as U;
        internal T GetObject(IGameElement _parent)
        {
            T _object = TryGetExistingObject(out T _obj) ? _obj : TryToCreateObject();
            PoolManager.ResetObj(_object, true, _parent);
            return _object;
        }

        private bool TryGetExistingObject(out T _obj)
        {
            foreach (T _objInPool in objectsInPool)
            {
                if (IsAvailable(_objInPool))
                {
                    _obj = _objInPool;
                    return true;
                }
            }
            _obj = default;
            return false;
        }

        private bool IsAvailable(T _obj)
        {
            if (_obj == null)
            {
                Debug.LogError("Pool object is null");
                return false;
            }
            return _obj.IsAvailable();
        }

        internal bool IsRefObjectNull() => referenceObject == null;

        private T TryToCreateObject()
        {
            if (referenceObject == null) "Reference object is null".LogError();
            if (creationMethod == null) return default;

            T _newObject = creationMethod.Invoke(referenceObject);
            Add(_newObject);
            return _newObject;
        }

        internal void CleanPool()
        {
            foreach (T _obj in this)
                PoolManager.GoBackToPool(_obj);
        }

        #region IList
        public int Count => ((ICollection<T>)objectsInPool).Count;
        public bool IsReadOnly => ((ICollection<T>)objectsInPool).IsReadOnly;
        public T this[int _index] { get => ((IList<T>)objectsInPool)[_index]; set => ((IList<T>)objectsInPool)[_index] = value; }
        public int IndexOf(T _item) => ((IList<T>)objectsInPool).IndexOf(_item);
        public void Insert(int _index, T _item) => ((IList<T>)objectsInPool).Insert(_index, _item);
        public void RemoveAt(int _index) => ((IList<T>)objectsInPool).RemoveAt(_index);
        public void Add(T _item) => ((ICollection<T>)objectsInPool).Add(_item);
        public void Clear() => ((ICollection<T>)objectsInPool).Clear();
        public bool Contains(T _item) => ((ICollection<T>)objectsInPool).Contains(_item);
        public void CopyTo(T[] _array, int _arrayIndex) => ((ICollection<T>)objectsInPool).CopyTo(_array, _arrayIndex);
        public bool Remove(T _item) => ((ICollection<T>)objectsInPool).Remove(_item);
        public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)objectsInPool).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)objectsInPool).GetEnumerator();
        #endregion
    }
}