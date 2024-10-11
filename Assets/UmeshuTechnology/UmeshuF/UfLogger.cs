using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Umeshu.Uf
{
    public static class UfLogger // Class name must finish by Logger to work, and methods must start with Log
    {

        private static readonly List<string> authorizedCategories = new();
        private static readonly List<string> unauthorizedCategories = new();
        internal static readonly List<string> newCategories = new();
        private static readonly List<string> alreadyLoggedLog = new();

        internal static void AddCategory(bool _authorized, string _category)
        {
            if (_authorized) authorizedCategories.Add(_category);
            else unauthorizedCategories.Add(_category);
        }
        internal static bool CanLogCategory(string _category)
        {
            if (authorizedCategories.Contains(_category)) return true;
            else if (!unauthorizedCategories.Contains(_category) && !newCategories.Contains(_category)) newCategories.Add(_category);
            return !unauthorizedCategories.Contains(_category);
        }

        #region Simple Log
        [HideInCallstack] public static void Log(this object _log, string _category = "", Object _context = null) => LogProxy(_log.ToNullableString(), Debug.Log, Debug.Log, _category, _context);
        [HideInCallstack] public static void Log(this object _log, Color _col, string _category = "", Object _context = null) => LogProxy(_log.ToNullableString().Color(_col), Debug.Log, Debug.Log, _category, _context);
        [HideInCallstack] public static void LogDesc(this object _log, string _desc, string _category = "", Object _context = null) => LogProxy(_desc + " : " + _log.ToNullableString(), Debug.Log, Debug.Log, _category, _context);
        [HideInCallstack] public static void LogDesc(this object _log, string _desc, Color _col, string _category = "", Object _context = null) => LogProxy((_desc + " : " + _log.ToNullableString()).Color(_col), Debug.Log, Debug.Log, _category, _context);
        [HideInCallstack] public static void LogWarning(this object _log, string _category = "", Object _context = null) => LogProxy(_log.ToNullableString(), Debug.LogWarning, Debug.LogWarning, _category, _context);
        [HideInCallstack] public static void LogError(this object _log, string _category = "", Object _context = null) => LogProxy(_log.ToNullableString(), Debug.LogError, Debug.LogError, _category, _context);
        [HideInCallstack] public static void LogCollection<T>(this IEnumerable<T> _values, string _category = "", Object _context = null) => LogCollectionProxy(_values, Debug.Log, Debug.Log, LineLogDefault, _category, _context);
        [HideInCallstack] public static void LogCollection<T>(this IEnumerable<T> _values, Color _col, string _category = "", Object _context = null) => LogCollectionProxy(_values, Debug.Log, Debug.Log, LineLogDefault, _category, _context, _col);
        [HideInCallstack] public static void LogWarningCollection<T>(this IEnumerable<T> _values, string _category = "", Object _context = null) => LogCollectionProxy(_values, Debug.LogWarning, Debug.LogWarning, LineLogDefault, _category, _context);
        [HideInCallstack] public static void LogErrorCollection<T>(this IEnumerable<T> _values, string _category = "", Object _context = null) => LogCollectionProxy(_values, Debug.LogError, Debug.LogError, LineLogDefault, _category, _context);
        [HideInCallstack] public static string ToNullableString(this object _object) => _object != null ? _object.ToString() : "null";
        [HideInCallstack] public static string GetLogID(this Object _obj) => _obj != null ? _obj.GetInstanceID().LogColoredInt() : "null";
        [HideInCallstack]
        public static void LogOnce(this object _log, string _category = "", Object _context = null)
        {
            string _logString = _log.ToNullableString();
            if (!alreadyLoggedLog.Contains(_logString))
            {
                alreadyLoggedLog.Add(_logString);
                _log.Log(_category, _context);
            }
        }

        [HideInCallstack] public static bool LogErrorIfFalse(this bool _condition, object _log, string _category = "", Object _context = null) => LogProxyIfFalse(_condition, () => _log.LogError(_category, _context), _category, _context);
        [HideInCallstack] public static bool LogErrorIfTrue(this bool _condition, object _log, string _category = "", Object _context = null) => LogProxyIfTrue(_condition, () => _log.LogError(_category, _context), _category, _context);
        [HideInCallstack] public static bool LogWarningIfFalse(this bool _condition, object _log, string _category = "", Object _context = null) => LogProxyIfFalse(_condition, () => _log.LogWarning(_category, _context), _category, _context);
        [HideInCallstack] public static bool LogWarningIfTrue(this bool _condition, object _log, string _category = "", Object _context = null) => LogProxyIfTrue(_condition, () => _log.LogWarning(_category, _context), _category, _context);


        [HideInCallstack]
        public static bool LogProxyIfFalse(this bool _condition, Action _action, string _category = "", Object _context = null)
        {
            if (!_condition) _action.Invoke();
            return _condition;
        }

        [HideInCallstack]
        public static bool LogProxyIfTrue(this bool _condition, Action _action, string _category = "", Object _context = null)
        {
            if (_condition) _action.Invoke();
            return _condition;
        }

        [HideInCallstack]
        public static bool LogIsIndexValid<T>(int _index, IEnumerable<T> _collection, string _category = "")
        {
            bool _valid = _index >= 0 && _index < _collection.Count();
            if (!_valid) $"Index {_index} is not valid for collection of size {_collection.Count()}".LogError(_category);
            return _valid;
        }

        [HideInCallstack]
        public static string ToCollectionString<T>(this IEnumerable<T> _values, Func<T, string> _lineLog = null, Color? _color = null)
        {
            string _finalText = "null";
            _lineLog ??= LineLogDefault;
            if (_values != null)
            {
                _finalText = $"Collection of type {typeof(T).Name.Bold()} and size {_values.Count().ToString().Bold()} :";
                if (_color.HasValue) _finalText = _finalText.Color(_color.Value);
                _finalText += "\n";
                int _index = 0;
                foreach (T _item in _values)
                {
                    _finalText += $"\n{$"{_index.RandomColor()}->".Bold()} {_lineLog.Invoke(_item)}";
                    _index++;
                }

                _finalText += "\n";
            }
            return _finalText;
        }

        [HideInCallstack]
        private static void LogCollectionProxy<T>(this IEnumerable<T> _values, Action<string> _logMethod, Action<string, Object> _logMethodWithContext, Func<T, string> _lineLog, string _category, Object _context, Color? _color = null) =>
            LogProxy(_values.ToCollectionString(_lineLog, _color), _logMethod, _logMethodWithContext, _category, _context);

        [HideInCallstack]
        private static void LogProxy(string _log, Action<string> _logMethod, Action<string, Object> _logMethodWithContext, string _category, Object _context)
        {
            bool _emptyCategory = String.IsNullOrEmpty(_category);
            if (_emptyCategory || CanLogCategory(_category))
            {
                string _inside = (_emptyCategory ? "" : ($"[{_category}] ").Bold().Size(8)) + _log;
                if (_context != null) _logMethodWithContext.Invoke(_inside, _context);
                else _logMethod.Invoke(_inside);
            }
        }

        private static string LineLogDefault<T>(T _item) => _item.ToNullableString();

#if UNITY_EDITOR
        [HideInCallstack] public static void LogCollectionHyperLinked<T>(this IEnumerable<T> _values, string _category = "", Object _context = null) where T : Object => LogCollectionProxy(_values, Debug.Log, Debug.Log, UfEditor.GetHyperLink, _category, _context);
        [HideInCallstack] public static void LogWarningCollectionHyperLinked<T>(this IEnumerable<T> _values, string _category = "", Object _context = null) where T : Object => LogCollectionProxy(_values, Debug.LogWarning, Debug.LogWarning, UfEditor.GetHyperLink, _category, _context);
        [HideInCallstack] public static void LogErrorCollectionHyperLinked<T>(this IEnumerable<T> _values, string _category = "", Object _context = null) where T : Object => LogCollectionProxy(_values, Debug.LogError, Debug.LogError, UfEditor.GetHyperLink, _category, _context);
#endif
        #endregion

        #region One Time Log
        private static readonly List<string> alreadyUsedKeys = new();

        private static bool Valid(string _key)
        {
            if (alreadyUsedKeys.Contains(_key)) return false;
            alreadyUsedKeys.Add(_key);
            return true;
        }
        [HideInCallstack]
        public static void Log(string _key, string _log)
        {
            if (!Valid(_key)) return;
            Debug.Log(_log);
        }
        [HideInCallstack]
        public static void LogWarning(string _key, string _log)
        {
            if (!Valid(_key)) return;
            Debug.LogWarning(_log);
        }
        [HideInCallstack]
        public static void LogError(string _key, string _log)
        {
            if (!Valid(_key)) return;
            Debug.LogError(_log);
        }
        [HideInCallstack] private static string GetMonoKey<T>(T _mono) where T : UnityEngine.Object => typeof(T).Name + _mono.GetInstanceID();
        [HideInCallstack] public static void LogOnce<T>(this T _mono, string _log) where T : UnityEngine.Object => Log(GetMonoKey(_mono), _log);
        [HideInCallstack] public static void LogOnce_Warning<T>(this T _mono, string _log) where T : UnityEngine.Object => LogWarning(GetMonoKey(_mono), _log);
        [HideInCallstack] public static void LogOnce_Error<T>(this T _mono, string _log) where T : UnityEngine.Object => LogError(GetMonoKey(_mono), _log);
        #endregion

#if UNITY_EDITOR
        public static void LogErrorWindow(this string _msg) => EditorUtility.DisplayDialog("Error", _msg, "Okay...");

#endif
    }
}
