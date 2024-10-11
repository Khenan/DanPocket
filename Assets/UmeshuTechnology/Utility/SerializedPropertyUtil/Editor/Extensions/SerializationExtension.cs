#if UNITY_EDITOR

using System;
using Umeshu.Uf;
using Umeshu.Utility;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
public static class SerializationExtension
{
    public static Type GetTypeOfProperty(this SerializedProperty _property) => _property.GetValue().GetType();
    public static object GetValue(this SerializedProperty _property) => _property.CleanAccess().GetUnderlyingValue();
    private static SerializedProperty CleanAccess(this SerializedProperty _property) => _property.IsArrayElement() ? _property.GetAccessToArrayElement() : _property;
    public static void Copy(this SerializedProperty _property, SerializedProperty _source)
    {
        if (_source.IsArrayElement()) _property.SetValue(_source.GetAccessToArrayElement());
        else _property.SetValue(_source.GetValue());
    }

    public static void SetValue(this SerializedProperty _property, object _value)
    {
        _property.serializedObject.Update();
        if (_property.IsArrayElement()) _property.SetArrayValue(_value);
        else _property.SetUnderlyingValue(_value);
        _property.serializedObject.ApplyModifiedProperties();
    }

    private static SerializedProperty GetAccessToArrayElement(this SerializedProperty _property)
    {
        int _index = GetIndex(_property.propertyPath);
        string _cleanedPath = GetArrayParentPath(_property.propertyPath);
        SerializedProperty _array = _property.serializedObject.FindProperty(_cleanedPath);
        return _array.GetArrayElementAtIndex(_index);
    }

    public static bool IsArrayElement(this SerializedProperty _property)
    {
        string[] _parts = _property.propertyPath.Split('.');
        return _parts.Length < 2 ? false : _parts[^2] == "Array";
    }

    public static bool IsArrayElement(this SerializedProperty _property, out int _index)
    {
        bool _isArray = _property.IsArrayElement();
        _index = _isArray ? _property.GetIndex() : -1;
        return _isArray;
    }

    private static void SetArrayValue(this SerializedProperty _property, object _value) => _property.GetAccessToArrayElement().managedReferenceValue = _value;

    private static string GetArrayParentPath(string _path)
    {
        string[] _pathParts = _path.Split('.');
        string _cleanPath = "";
        for (int _i = 0; _i < _pathParts.Length - 2; _i++)
        {
            _cleanPath += _pathParts[_i];
            if (_i + 1 < _pathParts.Length - 2) _cleanPath += '.';
        }
        return _cleanPath;
    }

    private static int GetIndex(this SerializedProperty _property) => GetIndex(_property.propertyPath);
    private static int GetIndex(string _path)
    {
        string _index = "";
        bool _started = false;
        for (int _i = _path.Length - 1; _i >= 0; _i--)
        {
            if (_path[_i] == ']') _started = true;
            else if (_path[_i] == '[') break;
            else if (_started)
            {
                _index += _path[_i];
            }
        }
        return int.TryParse(_index.Reversed(), out int _result) ? _result : -1;
    }

    public static Rect[] SplitRect(this Rect _rect, bool _withSpacing, int _nbEvenCut)
    {
        if (_nbEvenCut <= 0) return new Rect[] { _rect };

        float[] _cutsInPercentage = new float[_nbEvenCut];
        float _distanceBetweenCuts = 1f / (_nbEvenCut + 1);
        for (int _i = 0; _i < _cutsInPercentage.Length; _i++)
            _cutsInPercentage[_i] = (_i + 1) * _distanceBetweenCuts;

        return _rect.SplitRect(_withSpacing, _cutsInPercentage);
    }

    public static Rect[] SplitRect(this Rect _rect, bool _withSpacing, params float[] _cutsInPercentage)
    {
        Rect[] _rects = new Rect[_cutsInPercentage.Length + 1];
        float _totalSize = _rect.width;
        float _currentX = _rect.x;
        float _spacing = _withSpacing && _cutsInPercentage.Length > 0 ? 10 : 0;
        for (int _i = 0; _i < _rects.Length; _i++)
        {
            float _currentCut = _i == 0 ? _cutsInPercentage[_i] : _i == _cutsInPercentage.Length ? 1 - _cutsInPercentage[_i - 1] : _cutsInPercentage[_i] - _cutsInPercentage[_i - 1];
            bool _isFirstOrLast = _i == 0 || _i == _rects.Length - 1;
            float _spacingRemoved = _isFirstOrLast ? _spacing / 2f : _spacing;
            float _wantedSize = _totalSize * _currentCut - _spacingRemoved;
            _rects[_i] = new Rect(_currentX + _spacingRemoved / 2f, _rect.y, _wantedSize, _rect.height);
            _currentX += _wantedSize + _spacingRemoved;
        }
        return _rects;
    }

    public static void CustomPropertyDisplay(Rect _rect, SerializedProperty _property, GUIContent _content, bool _includeChildren, ArrayBasedSelectorType _arrayBasedSelectorType)
    {
        if (_arrayBasedSelectorType == ArrayBasedSelectorType.Default)
        {
            EditorGUI.PropertyField(_rect, _property, _content, _includeChildren);
            return;
        }
        bool _isNonFoldableClass = _property.propertyType != SerializedPropertyType.Generic;

        Rect[] _rects = _rect.SplitRect(_withSpacing: false, .33f);
        _rects[0].height = EditorGUIUtility.singleLineHeight;
        EditorGUI.LabelField(_rects[0], _content.text);

        if (_arrayBasedSelectorType == ArrayBasedSelectorType.FullDisplayed)
        {
            _property.isExpanded = true;
            _property.DrawAllProperties(_rects[1], _includeChildren, CustomPropertyDisplayNonfoldable);
        }
        else EditorGUI.PropertyField(_rects[1], _property, GUIContent.none, _includeChildren);
    }

    public static void CustomPropertyDisplayNonfoldable(Rect _rect, SerializedProperty _property, GUIContent _content, bool _includeChildren) => CustomPropertyDisplay(_rect, _property, _content, _includeChildren, ArrayBasedSelectorType.Default);

    public static void DrawAllProperties(this SerializedProperty _property, Rect _rect, bool _includeChildren = true, Action<Rect, SerializedProperty, GUIContent, bool> _propertyMethodDisplay = null)
    {
        int _indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        SerializedProperty _childProperty = _property.Copy();
        SerializedProperty _endProperty = _property.GetEndProperty();
        _rect.height = EditorGUIUtility.singleLineHeight;
        while (_childProperty.NextVisible(true) && !SerializedProperty.EqualContents(_childProperty, _endProperty))
        {
            if (_propertyMethodDisplay != null) _propertyMethodDisplay(_rect, _childProperty, new GUIContent(_childProperty.displayName), _includeChildren);
            else EditorGUI.PropertyField(_rect, _childProperty, new GUIContent(_childProperty.displayName), _includeChildren);
            _rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        EditorGUI.indentLevel = _indent;
    }
}

#endif