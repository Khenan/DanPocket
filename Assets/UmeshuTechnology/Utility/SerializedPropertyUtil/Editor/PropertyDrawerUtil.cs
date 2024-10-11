#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Umeshu.Utility
{
    public abstract class PropertyDrawerUtil : PropertyDrawer
    {
        protected SerializedProperty property;
        protected SerializedProperty GetProperty(string _name) => property.FindPropertyRelative(_name);
        public override float GetPropertyHeight(SerializedProperty _property, GUIContent _label)
        {
            return EditorGUI.GetPropertyHeight(_property, true);
        }
        public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
        {
            _property.serializedObject.Update();
            property = _property;
            EditorGUI.BeginProperty(_position, _label, _property);

            OnCustomGUI(ref _position, _property, _label);

            EditorGUI.EndProperty();
            _property.serializedObject.ApplyModifiedProperties();
        }

        public abstract void OnCustomGUI(ref Rect _position, SerializedProperty _property, GUIContent _label);
        protected Type GetGenerycType() => property.GetTypeOfProperty().GenericTypeArguments[0];
        protected void AddGenericTypeStringToLabel(GUIContent _label) => _label.text += $" <{GetGenerycType().Name}>";
    }
}
#endif