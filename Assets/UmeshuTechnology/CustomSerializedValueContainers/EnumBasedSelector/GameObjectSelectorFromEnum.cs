using System;
using UnityEngine;

namespace Umeshu.Utility
{
    public abstract class GameObjectSelectorFromPlayerIdentifier<T> : VarSelectorFromEnum<T, GameObject> where T : Enum
    {
        protected override void DoSelection() => values.DoMethodOnAll(Key, ActivateGoodObject);
        private void ActivateGoodObject(bool _isGoodObject, GameObject _gameObject)
        {
            if (_gameObject == null) return;
            _gameObject.SetActive(_isGoodObject);
        }
    }
}