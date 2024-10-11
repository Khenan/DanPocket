using System;
using UnityEngine;

namespace Umeshu.Utility
{
    public abstract class VarSelectorFromEnum<T, U> : MonoBehaviour where T : Enum
    {
        [SerializeField] protected EnumBasedSelector<T, U> values;
        protected abstract T Key { get; }
        protected abstract void DoSelection();
        private void OnEnable() => DoSelection();
    }
}