using System;
using System.Collections.Generic;
namespace Umeshu.Common
{
    public class UEvent<T> : UDelegate<Action<T>>
    {
        public UEvent() { }
        public UEvent(params Action<T>[] _listeners) : base(_listeners) { }
        public UEvent(Action<T> _listener) : base(_listener) { }

        public void Invoke(T _argument) => base.Invoke(_argument);

        public static UEvent<T> operator +(UEvent<T> _event, Action<T> _listener) => SubscribeTo(_event, _listener);
        public static UEvent<T> operator -(UEvent<T> _event, Action<T> _listener) => UnsubscribeFrom(_event, _listener);
    }

    public class UEvent : UDelegate<Action>
    {
        public UEvent() { }
        public UEvent(params Action[] _listeners) : base(_listeners) { }
        public UEvent(Action _listener) : base(_listener) { }

        public void Invoke() => Invoke(null);
        protected override void Invoke(Action _delegate, object _argument) => _delegate.Invoke();

        public static UEvent operator +(UEvent _event, Action _listener) => SubscribeTo(_event, _listener);
        public static UEvent operator -(UEvent _event, Action _listener) => UnsubscribeFrom(_event, _listener);
    }

    /// <summary>
    /// Base class for UEvents
    /// </summary>
    public abstract class UDelegate<T> where T : Delegate
    {
        public UDelegate() { }
        public UDelegate(params T[] _listeners)
        {
            Register(_listeners);
        }
        public UDelegate(T _listener)
        {
            Register(_listener);
        }

        private List<T> listeners = new();

        protected void Invoke(object _argument)
        {
            if (listeners?.Count == 0)
            {
                return;
            }

            List<int> _dirtyIndexes = new();
            for (int _i = 0; _i < listeners.Count; _i++)
            {
                try
                {
                    Invoke(listeners[_i], _argument);
                }
                catch (Exception _exc)
                {
                    _dirtyIndexes.Add(_i);
                    UnityEngine.Debug.LogWarning($"Could not trigger listener {listeners[_i]} ({_exc}) ; cleaning up.");
                }
            }
            Clean(_dirtyIndexes);
        }

        protected virtual void Invoke(T _delegate, object _argument)
        {
            _delegate.DynamicInvoke(_argument);
        }

        public bool IsRegistered(T _listener) => listeners.Contains(_listener);

        public void Register(params T[] _listeners)
        {
            for (int _i = 0; _i < _listeners.Length; _i++)
            {
                Register(_listeners[_i]);
            }
        }

        public void Register(T _listener)
        {
            if (IsRegistered(_listener))
            {
                return;
            }
            listeners.Add(_listener);
        }

        public void Unregister(params T[] _listeners)
        {
            for (int _i = 0; _i < _listeners.Length; _i++)
            {
                Unregister(_listeners[_i]);
            }
        }

        public void Unregister(T _listener)
        {
            listeners.Remove(_listener);
        }

        public void Clear() => listeners?.Clear();

        private void Clean(List<int> _dirtyIndexes)
        {
            for (int _i = _dirtyIndexes.Count - 1; _i >= 0; _i--)
            {
                listeners.RemoveAt(_dirtyIndexes[_i]);
            }
        }
        protected static U SubscribeTo<U, V>(U _uDelegate, V _listener) where U : UDelegate<V>, new() where V : Delegate
        {
            _uDelegate ??= new();
            _uDelegate.Register(_listener);
            return _uDelegate;
        }

        protected static U UnsubscribeFrom<U, V>(U _uDelegate, V _listener) where U : UDelegate<V>, new() where V : Delegate
        {
            _uDelegate ??= new();
            _uDelegate.Unregister(_listener);
            return _uDelegate;
        }
    }
}