using System;
using System.Collections.Generic;
using System.Reflection;

namespace Umeshu.Uf
{
    public static class UfReflection
    {
        public static List<T> GetAllVariableOfType<T>(this object _obj)
        {
            List<T> _list = new();
            FieldInfo[] _fields = _obj.GetType().UnderlyingSystemType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);

            foreach (FieldInfo _field in _fields)
            {
                if (_field.GetValue(_obj) is T _castedValue) _list.Add(_castedValue);
            }
            return _list;
        }

        public static Type[] GetSubTypes(this Type _type, bool _allAssemblies = false, bool _checkRawGenerics = false) => _allAssemblies ? GetSubTypes(_type, GetAllTypes(), _checkRawGenerics) : GetSubTypes(_type, _type.Assembly.GetTypes(), _checkRawGenerics);

        private static Type[] GetSubTypes(this Type _type, IEnumerable<Type> _types, bool _checkRawGenerics)
        {
            List<Type> _returnTypes = new();
            if (_checkRawGenerics && _type.IsGenericType)
            {
                foreach (Type _codeBaseType in _types)
                    if (_codeBaseType.InheritsFromRawGeneric(_type))
                        _returnTypes.Add(_codeBaseType);
            }
            else if (_type.IsClass)
            {
                foreach (Type _codeBaseType in _types)
                    if (_codeBaseType.InheritsFrom(_type))
                        _returnTypes.Add(_codeBaseType);
            }
            else if (_type.IsInterface)
            {
                foreach (Type _codeBaseType in _types)
                    if (_codeBaseType.ImplementInterface(_type))
                        _returnTypes.Add(_codeBaseType);
            }
            return _returnTypes.ToArray();
        }

        public static bool ImplementInterface(this Type _classType, Type _interfaceType)
        {
            foreach (Type _interface in _classType.GetInterfaces())
                if (_interface == _interfaceType)
                    return true;
            return false;
        }

        private static IEnumerable<Type> GetAllTypes()
        {
            foreach (Assembly _assembly in AppDomain.CurrentDomain.GetAssemblies())
                foreach (Type _type in _assembly.GetTypes()) yield return _type;
        }

        public static bool InheritsFrom(this Type _chekedType, Type _parent) => _chekedType.IsSubclassOf(_parent);

        public static bool InheritsFromRawGeneric(this Type _chekedType, Type _parent)
        {
            while (_chekedType != null && _chekedType != typeof(object))
            {
                Type _cur = _chekedType.IsGenericType ? _chekedType.GetGenericTypeDefinition() : _chekedType;
                if (_parent == _cur) return true;
                _chekedType = _chekedType.BaseType;
            }
            return false;
        }
    }
}
