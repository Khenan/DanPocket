#if UNITY_EDITOR

using Umeshu.Common;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(UVar<>))]
public class UVarUIE : PropertyDrawer
{

    public override float GetPropertyHeight(SerializedProperty _property, GUIContent _label)
    {
        SerializedProperty _resetValueProperty = _property.FindPropertyRelative(nameof(UVar<object>.Reset).ToLower());
        return EditorGUI.GetPropertyHeight(_resetValueProperty);
    }

    public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
    {
        if (Application.isPlaying)
        {
            SerializedProperty _resetValueProperty = _property.FindPropertyRelative(nameof(UVar<object>.Reset).ToLower());
            SerializedProperty _currValueProperty = _property.FindPropertyRelative(nameof(UVar<object>.Value).ToLower());

            Rect _labelPos = new(_position.x, _position.y, _position.width, _position.height);

            _position = EditorGUI.PrefixLabel(_labelPos, EditorGUIUtility.GetControlID(FocusType.Passive), _label);

            float _textSpace = 35;
            float _space = 5;
            float _floatWidth = (_position.width - _space * 3 - _textSpace * 2) / 2;

            _position.width = _textSpace;
            EditorGUI.LabelField(_position, "Value");

            _position.x += _position.width + _space;
            _position.width = _floatWidth;

            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            EditorGUI.PropertyField(_position, _currValueProperty, GUIContent.none);
            EditorGUI.EndDisabledGroup();

            _position.x += _position.width + _space;
            _position.width = _textSpace;
            EditorGUI.LabelField(_position, "Reset");

            _position.x += _position.width + _space;
            _position.width = _floatWidth;

            EditorGUI.PropertyField(_position, _resetValueProperty, GUIContent.none);
        }
        else
        {
            SerializedProperty _resetValueProperty = _property.FindPropertyRelative(nameof(UVar<object>.Reset).ToLower());
            EditorGUI.PropertyField(_position, _resetValueProperty, _label, true);
        }
    }
}

#endif