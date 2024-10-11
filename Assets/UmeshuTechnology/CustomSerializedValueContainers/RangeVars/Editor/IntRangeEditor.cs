#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;


namespace Umeshu.Utility
{

    [CustomPropertyDrawer(typeof(IntRange))]
    public class IntRangeEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty _property, GUIContent _label)
        {
            return base.GetPropertyHeight(_property, _label);
        }
        public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
        {
            SerializedProperty[] _properties = new SerializedProperty[] { _property.FindPropertyRelative(nameof(IntRange.Min).ToLower()), _property.FindPropertyRelative(nameof(IntRange.Max).ToLower()) };
            int[] _values = new int[] { _properties[0].intValue, _properties[1].intValue };
            EditorGUI.BeginProperty(_position, _label, _property);
            DisplayMultiIntField(_position, _properties, _values, _label);
            EditorGUI.EndProperty();
        }

        /// <summary>
        /// Display a int multi property that acts as a min max
        /// </summary>
        /// <param name="_position">Where to display</param>
        /// <param name="_properties">Array of two float properties to set the values of</param>
        /// <param name="_values">Array of two values</param>
        /// <param name="_label">Label to display</param>
        /// <param name="drawMode">Optionnal display options</param>
        public static void DisplayMultiIntField(Rect _position, SerializedProperty[] _properties, int[] _values, GUIContent _label)
        {
            DisplayMultiIntField(_position, ref _values, _label);
            for (int _i = 0; _i < 2; _i++) _properties[_i].intValue = _values[_i];
        }


        /// <summary>
        /// Display an int multi property that acts as a min max
        /// </summary>
        /// <param name="_position">Where to display</param>
        /// <param name="_values">Array of two values</param>
        /// <param name="_label">Label to display</param>
        /// <param name="drawMode">Optionnal display options</param>
        public static void DisplayMultiIntField(Rect _position, ref int[] _values, GUIContent _label)
        {
            int[] _old = new[] { _values[0], _values[1] };
            Rect _labelRect = new(_position.position, new Vector2(EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight));
            Rect _valuesRect = new(_labelRect.xMax, _position.position.y, _position.width - _labelRect.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(_labelRect, _label);
            EditorGUI.BeginChangeCheck();
            EditorGUI.MultiIntField(_valuesRect, FloatRangeEditor.content, _values);
            if (!EditorGUI.EndChangeCheck()) return;
            if (!TryTranslate(ref _values, _old))
                if (!EditorGUIUtility.editingTextField)
                    if (_values[0] > _values[1] && _values[0] != _values[1]) _values[1] = _values[0];
                    else if (_values[1] < _values[0]) _values[0] = _values[1];
        }
        public static bool TryTranslate(ref int[] _values, int[] _oldValues)
        {
            Event _event = Event.current;
            if (_event.alt)
            {
                if (!TryTranslate(ref _values, _oldValues, 0, 1))
                    TryTranslate(ref _values, _oldValues, 1, 0);
                return true;
            }
            return false;
        }
        private static bool TryTranslate(ref int[] _values, int[] _oldValues, int _index, int _other)
        {
            if (_oldValues[_index] != _values[_index])
            {
                int _dst = _values[_index] - _oldValues[_index];
                _values[_other] += _dst;
                return true;
            }
            return false;
        }
    }
}


#endif