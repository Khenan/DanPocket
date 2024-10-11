#if UNITY_EDITOR

using Umeshu.Uf;
using UnityEditor;
using UnityEngine;

namespace Umeshu.Utility
{
    [CustomPropertyDrawer(typeof(MinMaxRange))]
    public class MinMaxRangeEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty _property, GUIContent _label)
        {
            return GetPropertyHeight();
        }
        public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
        {
            MinMaxRange _range = attribute as MinMaxRange;
            SerializedProperty[] _properties = new SerializedProperty[] { _property.FindPropertyRelative(nameof(MinMaxRange.min)), _property.FindPropertyRelative(nameof(MinMaxRange.max)) };
            EditorGUI.BeginProperty(_position, _label, _property);
            DrawMinMaxRange(_position, _properties, _range, _label);
            EditorGUI.EndProperty();
        }
        public static void DrawMinMaxRange(Rect _position, SerializedProperty[] _properties, MinMaxRange _range, GUIContent _label, MultiPropertyDrawMode _drawMode = MultiPropertyDrawMode.all)
        {
            if (_properties == null || _properties[0] == null || _properties[1] == null) return;
            if (_properties[0].propertyType == SerializedPropertyType.Float && _properties[1].propertyType == SerializedPropertyType.Float)
                DrawMinMaxRange(_position, _properties, _range.min, _range.max, _label);
            else if (_properties[0].propertyType == SerializedPropertyType.Integer && _properties[1].propertyType == SerializedPropertyType.Integer)
                DrawMinMaxRange(_position, _properties, (int)_range.min, (int)_range.max, _label);
        }

        public static void DrawMinMaxRange(Rect _position, SerializedProperty[] _properties, float _min, float _max, GUIContent _label, MultiPropertyDrawMode _drawMode = MultiPropertyDrawMode.all)
        {
            float[] _values = new float[2];
            for (int _i = 0; _i < 2; _i++) _values[_i] = _properties[_i].floatValue;
            DrawMinMaxRange(_position, ref _values, _min, _max, _label, _drawMode);
            for (int _i = 0; _i < 2; _i++) _properties[_i].floatValue = _values[_i];
        }

        public static void DrawMinMaxRange(Rect _position, SerializedProperty[] _properties, int _min, int _max, GUIContent _label, MultiPropertyDrawMode _drawMode = MultiPropertyDrawMode.all)
        {
            int[] _values = new int[2];
            for (int _i = 0; _i < 2; _i++) _values[_i] = _properties[_i].intValue;
            DrawMinMaxRange(_position, ref _values, _min, _max, _label, _drawMode);
            for (int _i = 0; _i < 2; _i++) _properties[_i].intValue = _values[_i];
        }

        public static void DrawMinMaxRange(Rect _position, ref int[] _values, int _min, int _max, GUIContent _label, MultiPropertyDrawMode _drawMode = MultiPropertyDrawMode.all)
        {
            float[] _floatValues;
            DrawLabel(_position, _label);
            if (_drawMode.HasFlag(MultiPropertyDrawMode.values))
            {
                IntRangeEditor.DisplayMultiIntField(_position, ref _values, GUIContent.none);
                _position.yMin += EditorGUIUtility.singleLineHeight * 1.15f;
                _floatValues = _values.Convert<int, float>();
                DrawMinMax(_position, ref _floatValues, _min, _max, _drawMode);
            }
            else
            {
                _floatValues = _values.Convert<int, float>();
                DrawMinMax(_position, ref _floatValues, _min, _max, _drawMode);
            }
            _values = _floatValues.Convert<float, int>();
        }

        public static void DrawMinMaxRange(Rect _position, ref float[] _values, float _min, float _max, GUIContent _label, MultiPropertyDrawMode _drawMode = MultiPropertyDrawMode.limits)
        {
            DrawLabel(_position, _label);
            if (_drawMode.HasFlag(MultiPropertyDrawMode.values))
            {
                FloatRangeEditor.DisplayMultiFloatField(_position, ref _values, GUIContent.none);
                _position.yMin += EditorGUIUtility.singleLineHeight * 1.15f;
                DrawMinMax(_position, ref _values, _min, _max, _drawMode);
            }
            else
            {
                DrawMinMax(_position, ref _values, _min, _max, _drawMode);
            }
        }
        private static void DrawLabel(Rect _position, GUIContent _label) => EditorGUI.LabelField(new Rect(_position.position, new Vector2(EditorGUIUtility.labelWidth - EditorGUI.indentLevel * 15, EditorGUIUtility.singleLineHeight)), _label);

        private static void DrawMinMax(Rect _position, ref float[] _values, float _min, float _max, MultiPropertyDrawMode _drawMode)
        {
            EditorGUI.BeginChangeCheck();
            if (_drawMode.HasFlag(MultiPropertyDrawMode.limits))
            {
                float _numbWidth = EditorGUIUtility.fieldWidth;
                float _offset = EditorGUI.indentLevel * 15;
                Rect _leftNumb = new(_position.position + Vector2.right * (EditorGUIUtility.labelWidth - _offset), new Vector2(_numbWidth, EditorGUIUtility.singleLineHeight));
                Rect _sliderRect = new(_leftNumb.position + Vector2.right * _leftNumb.width, new(_position.width - EditorGUIUtility.labelWidth - 2 * _numbWidth + _offset, EditorGUIUtility.singleLineHeight));
                Rect _rightNumb = new(_sliderRect.position + Vector2.right * _sliderRect.width, _leftNumb.size);
                GUI.enabled = false;
                EditorGUI.FloatField(_leftNumb, _min);
                GUI.enabled = true;
                EditorGUI.MinMaxSlider(_sliderRect, ref _values[0], ref _values[1], _min, _max);
                GUI.enabled = false;
                EditorGUI.FloatField(_rightNumb, _max);
                GUI.enabled = true;
            }
            else
            {
                Rect _sliderRect = new(_position.position + Vector2.right * EditorGUIUtility.labelWidth, new(_position.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight));
                EditorGUI.MinMaxSlider(_sliderRect, ref _values[0], ref _values[1], _min, _max);
            }
            if (EditorGUI.EndChangeCheck())
                EditorGUI.FocusTextInControl(null);

            for (int _i = 0; _i < 2; _i++)
            {
                _values[_i] = Mathf.Clamp(_values[_i], _min, _max);
            }
        }

        public static Rect GetRect(Vector2 _position, float _width, MultiPropertyDrawMode _drawMode)
        => new(_position, new(_width, GetPropertyHeight(_drawMode)));
        public static float GetPropertyHeight(MultiPropertyDrawMode _drawMode = MultiPropertyDrawMode.all)
        => _drawMode.HasFlag(MultiPropertyDrawMode.values) ? 2 * EditorGUIUtility.singleLineHeight + .35f * EditorGUIUtility.singleLineHeight : EditorGUIUtility.singleLineHeight;
    }
    [System.Flags]
    public enum MultiPropertyDrawMode
    {
        values = 1,
        limits = 2,
        all = values | limits
    }
}

#endif