using System;

namespace Umeshu.Utility
{
    public abstract class OneShotDelegate<T> where T : Delegate
    {
        protected abstract SecuredDelegate<T> SecuredDelegate { get; }
        protected void OnPostInvoke() => SecuredDelegate.SetAllowed(false);
    }

    public class OneShotMethod<T> : OneShotDelegate<Action<T>>
    {
        public OneShotMethod(SecuredMethod<T> _method)
        {
            method = _method;
        }
        public OneShotMethod(Action<T> _method)
        {
            method = new SecuredMethod<T>(_method, true);
        }
        protected override SecuredDelegate<Action<T>> SecuredDelegate => method;
        protected SecuredMethod<T> method;
        public void OneShotInvoke(T _value)
        {
            method.Invoke(_value);
            OnPostInvoke();
        }
        public void Reset() => method.SetAllowed(true);
    }

    public class OneShotMethod : OneShotDelegate<Action>
    {
        public OneShotMethod(SecuredMethod _method)
        {
            method = _method;
        }
        public OneShotMethod(Action _method)
        {
            method = new SecuredMethod(_method, true);
        }
        protected override SecuredDelegate<Action> SecuredDelegate => method;
        protected SecuredMethod method;
        public void OneShotInvoke()
        {
            method.Invoke();
            OnPostInvoke();
        }
    }
}
