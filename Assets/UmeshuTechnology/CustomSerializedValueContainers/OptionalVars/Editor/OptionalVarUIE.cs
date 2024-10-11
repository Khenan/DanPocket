#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Umeshu.Utility
{

    [CustomPropertyDrawer(typeof(OptionalVar<>))]
    public class OptionalVarUIE : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty _property, GUIContent _label)
        {
            SerializedProperty _valueProperty = _property.FindPropertyRelative(nameof(OptionalVar<object>.Value).ToLower());
            return EditorGUI.GetPropertyHeight(_valueProperty);
        }

        public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
        {
            SerializedProperty _valueProperty = _property.FindPropertyRelative(nameof(OptionalVar<object>.Value).ToLower());
            SerializedProperty _enabledProperty = _property.FindPropertyRelative(nameof(OptionalVar<object>.Enabled).ToLower());

            EditorGUI.BeginProperty(_position, _label, _property);
            _position.width -= 24;
            EditorGUI.BeginDisabledGroup(!_enabledProperty.boolValue);
            EditorGUI.PropertyField(_position, _valueProperty, _label, true);
            EditorGUI.EndDisabledGroup();

            int _indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            _position.x += _position.width + 24;
            _position.width = _position.height = EditorGUI.GetPropertyHeight(_enabledProperty);
            _position.x -= _position.width;
            EditorGUI.PropertyField(_position, _enabledProperty, GUIContent.none);
            EditorGUI.indentLevel = _indent;
            EditorGUI.EndProperty();
        }
    }
}
#endif