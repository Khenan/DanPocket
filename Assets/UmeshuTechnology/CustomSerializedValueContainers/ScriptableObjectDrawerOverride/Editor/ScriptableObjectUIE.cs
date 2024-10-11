#if UNITY_EDITOR

using Umeshu.Uf;
using Umeshu.USystem;
using Umeshu.Utility;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEditor.PlayerSettings;

[CustomPropertyDrawer(typeof(ScriptableObject), true)]
public class ScriptableObjectUIE : PropertyDrawer
{
    public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
    {
        bool _noObjectValue = _property.objectReferenceValue == null;
        bool _hasObject = !_noObjectValue;

        float _buttonSize = Mathf.Min(80, _position.width * 0.2f);
        if (_noObjectValue) _buttonSize *= 2.5f;
        _position.width -= _buttonSize;

        EditorGUI.PropertyField(_position, _property, _label);

        _position.x += _position.width;
        _position.width = _buttonSize;

        if (_hasObject)
        {
            if (GUI.Button(_position, new GUIContent("Open"))) CustomWindow.CreateWindow(_property.objectReferenceValue);
        }
        else if (_property.GetUnderlyingType().IsAbstract)
        {
            EditorGUI.LabelField(_position, new GUIContent("Abstract"), EditorStyles.helpBox);
        }
        else
        {
            Rect[] _rects = _position.SplitRect(_withSpacing: false, 0.6f);

            if (GUI.Button(_rects[0], new GUIContent("Find Existing")))
            {
                _property.objectReferenceValue = UfEditor.GetAssetOfType(_property.GetUnderlyingType());
                _property.serializedObject.ApplyModifiedProperties();
            }

            if (GUI.Button(_rects[1], new GUIContent("Create")))
            {
                _property.objectReferenceValue = UfEditor.CreateScritableObjectInProject(_property.GetUnderlyingType());
                _property.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}

#endif