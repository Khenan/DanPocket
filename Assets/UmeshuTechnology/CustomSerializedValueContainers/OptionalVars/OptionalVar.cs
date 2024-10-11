using System;
using UnityEngine;

namespace Umeshu.Utility
{
    [Serializable]
    public struct OptionalVar<T>
    {
        [SerializeField] private bool enabled;
        [SerializeField] private T value;

        public bool Enabled => enabled;
        public T Value { get => value; set => this.value = value; }

        public void SetEnabled(bool _newState)
        {
            enabled = _newState;
        }

        public void Use(T _newValue)
        {
            Value = _newValue;
            enabled = true;
        }

        public OptionalVar(T _initialValue)
        {
            enabled = true;
            value = _initialValue;
        }

        public OptionalVar(bool _initialEnabled)
        {
            enabled = _initialEnabled;
            value = default;
        }

        public OptionalVar(T _initialValue, bool _initialEnabled)
        {
            enabled = _initialEnabled;
            value = _initialValue;
        }

        public static implicit operator T(OptionalVar<T> _v) => _v.Value;
    }

    public static class OptionalVarExtensions
    {
#nullable enable
        public static OptionalVar<T> ToOptionalVar<T>(this T? _value) where T : struct => new(_value ?? default, _value.HasValue);
#nullable restore
    }

}
