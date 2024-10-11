using Umeshu.Common;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(UVarList<>))]
public class UVarListPropertyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty _property, GUIContent _label)
    {
        if (Application.isPlaying)
        {
            return EditorGUI.GetPropertyHeight(_property, true);
        }
        SerializedProperty _resetValueProperty = _property.FindPropertyRelative(nameof(UVarList<object>.Reset).ToLower());

        return EditorGUI.GetPropertyHeight(_resetValueProperty, true) + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
    }

    public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
    {
        SerializedProperty _resetValueProperty = _property.FindPropertyRelative(nameof(UVarList<object>.Reset).ToLower());
        if (Application.isPlaying)
        {
            SerializedProperty _currValueProperty = _property.FindPropertyRelative(nameof(UVar<object>.Value).ToLower());
            _position.height = EditorGUI.GetPropertyHeight(_currValueProperty, true);
            EditorGUI.PropertyField(_position, _currValueProperty, true);

            _position.y += _position.height;
            _position.height = EditorGUI.GetPropertyHeight(_resetValueProperty, true);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(_position, _resetValueProperty, true);
            EditorGUI.EndDisabledGroup();
        }
        else
        {
            EditorGUI.PropertyField(_position, _resetValueProperty, true);
        }
    }
}
#endif