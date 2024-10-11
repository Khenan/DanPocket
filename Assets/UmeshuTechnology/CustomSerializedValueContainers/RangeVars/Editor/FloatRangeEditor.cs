#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Umeshu.Utility
{
    [CustomPropertyDrawer(typeof(FloatRange))]
    public class FloatRangeEditor : PropertyDrawer
    {
        public static readonly GUIContent[] content = new GUIContent[] { new(nameof(FloatRange.Min)), new(nameof(FloatRange.Max)) };
        public override float GetPropertyHeight(SerializedProperty _property, GUIContent _label)
        {
            return base.GetPropertyHeight(_property, _label);
        }
        public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
        {
            SerializedProperty[] _properties = new SerializedProperty[] { _property.FindPropertyRelative(nameof(FloatRange.Min).ToLower()), _property.FindPropertyRelative(nameof(FloatRange.Max).ToLower()) };
            float[] _values = new float[] { _properties[0].floatValue, _properties[1].floatValue };

            EditorGUI.BeginProperty(_position, _label, _property);
            DisplayMultiFloatField(_position, _properties, _values, _label);
            EditorGUI.EndProperty();
        }

        /// <summary>
        /// Display a float multi property that acts as a min max
        /// </summary>
        /// <param name="_position">Where to display</param>
        /// <param name="_properties">Array of two float properties to set the values of</param>
        /// <param name="_values">Array of two values</param>
        /// <param name="_label">Label to display</param>
        /// <param name="drawMode">Optionnal display options</param>
        public static void DisplayMultiFloatField(Rect _position, SerializedProperty[] _properties, float[] _values, GUIContent _label)
        {
            DisplayMultiFloatField(_position, ref _values, _label);
            for (int _i = 0; _i < 2; _i++) _properties[_i].floatValue = _values[_i];
        }

        /// <summary>
        /// Display a float multi property that acts as a min max
        /// </summary>
        /// <param name="_position">Where to display</param>
        /// <param name="_values">Array of two values</param>
        /// <param name="_label">Label to display</param>
        public static void DisplayMultiFloatField(Rect _position, ref float[] _values, GUIContent _label)
        {
            float[] _old = new[] { _values[0], _values[1] };
            Rect _labelRect = new(_position.position, new Vector2(EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight));
            Rect _valuesRect = new(_labelRect.xMax, _position.position.y, _position.width - _labelRect.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(_labelRect, _label);
            EditorGUI.BeginChangeCheck();
            EditorGUI.MultiFloatField(_valuesRect, content, _values);
            Event _event = Event.current;
            if (!EditorGUI.EndChangeCheck()) return;
            if (_event.alt)
            {
                if (!TryTranslate(ref _values, _old, 0, 1)) TryTranslate(ref _values, _old, 1, 0);
            }
            else if (!EditorGUIUtility.editingTextField)
                if (_values[0] > _values[1] && _values[0] != _old[0]) _values[1] = _values[0];
                else if (_values[1] < _values[0]) _values[0] = _values[1];
        }

        private static bool TryTranslate(ref float[] _values, float[] _oldValues, int _index, int _other)
        {
            if (_oldValues[_index] != _values[_index])
            {
                float _dst = _values[_index] - _oldValues[_index];
                _values[_other] += _dst;
                return true;
            }
            return false;
        }
    }


}

#endif