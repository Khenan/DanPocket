using System;

namespace Umeshu.Utility
{
    public abstract class SecuredDelegate<T> where T : Delegate
    {
        protected T action;
        protected IAuthorizer authorizer;

        public SecuredDelegate(T _action, IAuthorizer _authorizer)
        {
            action = _action;
            authorizer = _authorizer;
        }
        public SecuredDelegate(T _action, bool _isAllowed) : this(_action, IAuthorizer.Create(_isAllowed)) { }
        public SecuredDelegate(T _action, Func<bool> _isAllowedMethod) : this(_action, IAuthorizer.Create(_isAllowedMethod)) { }


        public bool IsMethodAllowed() => authorizer?.IsAllowed ?? true;

        public bool TrySetAllowed(bool _value)
        {
            if (authorizer is AuthorizerToggle _toggle)
            {
                _toggle.IsAllowed = _value;
                return true;
            }
            return false;
        }

        public void SetAllowed(bool _value)
        {
            if (authorizer is AuthorizerToggle _toggle)
            {
                _toggle.IsAllowed = _value;
            }
            else
            {
                authorizer = IAuthorizer.Create(_value);
            }
        }

        public void SetIsAllowedMethod(Func<bool> _isAllowed)
        {
            authorizer = IAuthorizer.Create(_isAllowed);
        }
    }

    public class SecuredMethod<T> : SecuredDelegate<Action<T>>
    {
        public SecuredMethod(Action<T> _action, IAuthorizer _isMethodAllowed) : base(_action, _isMethodAllowed) { }
        public SecuredMethod(Action<T> _action, bool _isAllowed) : base(_action, _isAllowed) { }
        public SecuredMethod(Action<T> _action, Func<bool> _isAllowedMethod) : base(_action, _isAllowedMethod) { }

        public void Invoke(T _arg)
        {
            if (!IsMethodAllowed())
            {
                return;
            }
            action?.Invoke(_arg);
        }
    }

    public class SecuredMethod : SecuredDelegate<Action>
    {
        public SecuredMethod(Action _action, IAuthorizer _isMethodAllowed) : base(_action, _isMethodAllowed)
        {
        }
        public SecuredMethod(Action _action, bool _isAllowed) : base(_action, _isAllowed) { }
        public SecuredMethod(Action _action, Func<bool> _isAllowedMethod) : base(_action, _isAllowedMethod) { }

        public void Invoke()
        {
            if (!IsMethodAllowed())
            {
                return;
            }
            action?.Invoke();
        }
    }

    public interface IAuthorizer
    {
        public bool IsAllowed { get; }

        public static IAuthorizer Create(bool _value) => new AuthorizerToggle(_value);
        public static IAuthorizer Create(Func<bool> _isAllowedMethod) => new AuthorizerMethod(_isAllowedMethod);
    }

    public struct AuthorizerToggle : IAuthorizer
    {
        public AuthorizerToggle(bool _isAllowed)
        {
            IsAllowed = _isAllowed;
        }
        public bool IsAllowed { get; set; }
    }

    public struct AuthorizerMethod : IAuthorizer
    {
        public AuthorizerMethod(Func<bool> _isAllowed)
        {
            isAllowed = _isAllowed;
        }
        public bool IsAllowed => isAllowed?.Invoke() ?? true;
        public readonly Func<bool> isAllowed;
    }
}
