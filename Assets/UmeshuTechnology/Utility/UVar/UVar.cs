using System;
using Umeshu.Uf;
using UnityEngine;
namespace Umeshu.Common
{
    [Serializable]
    public class UVar<T> : IUVar
    {
        [SerializeField] private T value;
        [SerializeField] private T reset;
        [SerializeField] private UEvent<T> onValueChange = new();
        public UEvent<T> OnValueChange => onValueChange;
        public bool debugVar;

        public UVar() : this(default) { }

        public UVar(T _resetValue, bool _debugVar = false)
        {
            this.value = _resetValue;
            this.reset = _resetValue;
            this.debugVar = _debugVar;
        }

        public T Value
        {
            get => value;
            set
            {
                onValueChange?.Invoke(value);
                this.value = value;
                if (debugVar)
                {
                    Debug.Log("Value is set to " + this.value);
                }
            }
        }
        public T Reset
        {
            get => reset;
            set
            {
                reset = value;
                if (debugVar)
                {
                    Debug.Log("Reset value is set to " + reset);
                }
            }
        }

        public void ResetVar()
        {
            if (debugVar)
            {
                Debug.Log("Reset value from " + Value + " to " + Reset);
            }
            onValueChange.Clear();
            Value = Reset;
        }

        public static implicit operator T(UVar<T> _uVar) => _uVar is null ? default : _uVar.Value;
        public override string ToString() => $"UVar<{typeof(T).Name}> = {Value}";
    }

    public static class UVarExtension
    {
        public static void ResetVars(params object[] _objects)
        {
            foreach (object _object in _objects)
            {
                _object.ResetVars();
            }
        }

        public static void ResetVars(this object _obj)
        {
            foreach (IUVar _uVar in _obj.GetAllVariableOfType<IUVar>())
            {
                _uVar.ResetVar();
            }
        }
    }


    public interface IUVar
    {
        public void ResetVar();
    }
}