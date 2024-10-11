#if UNITY_EDITOR

using System;
using Umeshu.Uf;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace Umeshu.Utility
{

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
    public class Selector_FoldableUnfoldable : Attribute { }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
    public class Selector_FullDisplayed : Attribute { }

    public enum ArrayBasedSelectorType
    {
        Default,
        FoldableUnfoldable,
        FullDisplayed
    }

    [CustomPropertyDrawer(typeof(AbstractArrayBasedSelector<,>), true)]
    public class ArrayBasedSelectorUIE : PropertyDrawerUtil
    {
        private double timeSinceLastUpdate = 0;
        private ReorderableList reorderableList;
        private ArrayBasedSelectorType arrayBasedSelectorType;

        private const string ENTRY_KEY_PROPERTY = nameof(ArrayBasedSelectorEntry<object, object>.key);
        private const string ENTRY_VALUE_PROPERTY = nameof(ArrayBasedSelectorEntry<object, object>.value);
        private const string ENTRY_ERROR_PROPERTY = nameof(ArrayBasedSelectorEntry<object, object>.error);
        private const string ENTRIES_PROPERTY = nameof(ArrayBasedSelector<object, object>.entries);

        public override void OnCustomGUI(ref Rect _position, SerializedProperty _property, GUIContent _label)
        {
            if (timeSinceLastUpdate + 1 < EditorApplication.timeSinceStartup)
            {
                timeSinceLastUpdate = EditorApplication.timeSinceStartup;
                ((IArrayBasedSelector)_property.GetValue()).UpdateDictionaryKeys();
            }

            if (reorderableList == null) CreateReorderableList(_property);
            reorderableList.DoList(_position);
        }

        #region List Methods

        private void CreateReorderableList(SerializedProperty _property)
        {
            SerializedProperty _listProperty = GetValuesFromSerializedDictionary(_property);
            reorderableList = new(_listProperty.serializedObject, _listProperty, false, true, false, false)
            {
                drawHeaderCallback = DrawHeader,
                elementHeightCallback = GetKeyValuePairHeight,
                drawElementCallback = DrawListItems,
                drawFooterCallback = DrawFooter
            };
            _listProperty.arraySize = GetKeys(_property).Length;

            arrayBasedSelectorType = ArrayBasedSelectorType.Default;
            foreach (System.Reflection.CustomAttributeData _attributeData in fieldInfo.CustomAttributes)
            {
                if (_attributeData.AttributeType == typeof(Selector_FoldableUnfoldable))
                {
                    arrayBasedSelectorType = ArrayBasedSelectorType.FoldableUnfoldable;
                    return;
                }
                if (_attributeData.AttributeType == typeof(Selector_FullDisplayed))
                {
                    arrayBasedSelectorType = ArrayBasedSelectorType.FullDisplayed;
                    return;
                }
            }
        }

        private void DrawFooter(Rect _rect)
        {
            int _nbButton = 1;
            IArrayBasedSelector _arrayBasedSelector = (IArrayBasedSelector)property.GetValue();
            bool _hasUnlinkedKeys = _arrayBasedSelector.HasUnlinkedKeys();
            if (_hasUnlinkedKeys) _nbButton++;

            bool _hasKeysInWrongOrder = !_arrayBasedSelector.AreKeysInSameOrderAsCollection();
            if (_hasKeysInWrongOrder) _nbButton++;

            if (_nbButton == 0) return;

            float _wantedSizePerButton = Mathf.Max(150, _rect.width / 3f);

            float _wantedSize = _wantedSizePerButton * _nbButton;
            _rect.x += _rect.width - _wantedSize;
            _rect.width = _wantedSize;

            Rect _usedRect = _rect;

            Rect[] _rects = _rect.SplitRect(_withSpacing: true, _nbEvenCut: _nbButton - 1);
            int _currRectIndex = 0;

            const bool _displaySetDirtyButton = true;
            if (_displaySetDirtyButton)
            {
                if (_nbButton > 1)
                {
                    _usedRect = _rects[_currRectIndex];
                    _currRectIndex++;
                }
                if (GUI.Button(_usedRect, "Set Dirty"))
                {
                    EditorUtility.SetDirty(property.serializedObject.targetObject);
                }
            }

            if (_hasKeysInWrongOrder)
            {
                if (_nbButton > 1)
                {
                    _usedRect = _rects[_currRectIndex];
                    _currRectIndex++;
                }
                if (GUI.Button(_usedRect, nameof(ArrayBasedSelector<object, object>.MatchCollectionOrder)))
                {
                    _arrayBasedSelector.MatchCollectionOrder();
                    EditorUtility.SetDirty(property.serializedObject.targetObject);
                }
            }

            if (_hasUnlinkedKeys)
            {
                if (_nbButton > 1)
                {
                    _usedRect = _rects[_currRectIndex];
                    _currRectIndex++;
                }
                if (GUI.Button(_usedRect, nameof(ArrayBasedSelector<object, object>.RemoveUnlinkedKeys)))
                {
                    if (!EditorUtility.DisplayDialog("Remove Unlinked Keys", "Are you sure you want to remove unlinked keys?", "Yes", "No")) return;
                    _arrayBasedSelector.RemoveUnlinkedKeys();
                    EditorUtility.SetDirty(property.serializedObject.targetObject);
                }
            }
        }

        private void DrawHeader(Rect _rect) => EditorGUI.LabelField(_rect, new GUIContent($"{property.displayName} - {GetGenerycType()}"), EditorStyles.boldLabel);

        private void DrawListItems(Rect _rect, int _index, bool _isActive, bool _isFocused)
        {
            SerializedProperty _element = reorderableList.serializedProperty.GetArrayElementAtIndex(_index);
            _element.isExpanded = true;
            SerializedProperty _key = _element.FindPropertyRelative(ENTRY_KEY_PROPERTY);
            SerializedProperty _value = _element.FindPropertyRelative(ENTRY_VALUE_PROPERTY);
            SerializedProperty _error = _element.FindPropertyRelative(ENTRY_ERROR_PROPERTY);

            Rect _propertyRect = _rect;
            if (_error.boolValue)
            {
                _propertyRect.width -= EditorGUIUtility.singleLineHeight;
                _propertyRect.x += EditorGUIUtility.singleLineHeight;

                Rect _errorPictogramRect = new(_rect.x, _rect.y, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(_errorPictogramRect, EditorGUIUtility.IconContent("console.erroricon.sml"));
            }

            SerializationExtension.CustomPropertyDisplay(_propertyRect, _value, new GUIContent(_key.stringValue), true, arrayBasedSelectorType);
        }

        private float GetKeyValuePairHeight(int _index)
        {
            SerializedProperty _element = reorderableList.serializedProperty.GetArrayElementAtIndex(_index);
            SerializedProperty _value = _element.FindPropertyRelative(ENTRY_VALUE_PROPERTY);
            return arrayBasedSelectorType.IsOneOf(ArrayBasedSelectorType.Default, ArrayBasedSelectorType.FoldableUnfoldable) ? EditorGUI.GetPropertyHeight(_value, true) : EditorGUI.GetPropertyHeight(_value, true) - EditorGUIUtility.singleLineHeight;
        }

        #endregion

        #region Utilities

        public string[] GetKeys(SerializedProperty _property)
        {
            SerializedProperty _dicProperty = GetValuesFromSerializedDictionary(_property);
            string[] _keys = new string[_dicProperty.arraySize];
            for (int _i = 0; _i < _keys.Length; _i++)
            {
                SerializedProperty _element = _dicProperty.GetArrayElementAtIndex(_i);
                _keys[_i] = _element.FindPropertyRelative(ENTRY_KEY_PROPERTY).stringValue;
            }
            return _keys;
        }

        public override float GetPropertyHeight(SerializedProperty _property, GUIContent _label)
        {
            if (reorderableList == null) CreateReorderableList(_property);
            return reorderableList.GetHeight();
        }

        private static SerializedProperty GetValuesFromSerializedDictionary(SerializedProperty _property) => _property.FindPropertyRelative(ENTRIES_PROPERTY);

        #endregion

    }
}

#endif