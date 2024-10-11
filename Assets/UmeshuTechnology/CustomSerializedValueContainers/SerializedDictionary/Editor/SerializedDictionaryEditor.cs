#if UNITY_EDITOR

using Umeshu.Uf;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Umeshu.Utility
{
    [CustomPropertyDrawer(typeof(SerializedDictionary<,>), true)]
    public class SerializedDictionaryEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty _property, GUIContent _label)
        {
            _property.isExpanded = true; //since we display the list inside of the property and the list is expandable by itself, we want to make sure the property is always expanded
            return EditorGUI.GetPropertyHeight(_property, _label, true) - EditorGUIUtility.singleLineHeight + (ContainsDuplicateKeys(_property) ? EditorGUIUtility.singleLineHeight : 0);
        }

        public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
        {
            if (ContainsDuplicateKeys(_property))
            {
                Rect _helpBox = new(_position.xMin, _position.position.y, _position.width, EditorGUIUtility.singleLineHeight);
                _position.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.HelpBox(_helpBox, $"Duplicate keys are present in the {_label.text.Quote()} dictionary.", MessageType.Error);
            }
            if (_property.GetUnderlyingType().IsSubclassOf(typeof(SerializedDictionary<,>)))
            {
                EditorGUI.PropertyField(_position, _property, _label, true);
            }
            else
            {
                EditorGUI.PropertyField(_position, _property.FindPropertyRelative(nameof(SerializedDictionary<object, object>.Values).ToLower()), _label, true);
            }
        }

        public bool ContainsDuplicateKeys(SerializedProperty _property)
        {
            SerializedProperty _values = _property.FindPropertyRelative(nameof(SerializedDictionary<object, object>.Values).ToLower());
            SerializedProperty[] _keys = new SerializedProperty[_values.arraySize];
            for (int _i = 0; _i < _keys.Length; _i++)
            {
                _keys[_i] ??= GetKey(_i);
                for (int _j = _i + 1; _j < _keys.Length; _j++)
                {
                    _keys[_j] ??= GetKey(_j);
                    if (SerializedProperty.DataEquals(_keys[_i], _keys[_j])) return true;
                }
            }
            return false;
            SerializedProperty GetKey(int _index) => _values.GetArrayElementAtIndex(_index).FindPropertyRelative(nameof(SerializedDictionary<object, object>.SerializedDictionaryKeyValuePair.key));
        }
    }
}

#endif
