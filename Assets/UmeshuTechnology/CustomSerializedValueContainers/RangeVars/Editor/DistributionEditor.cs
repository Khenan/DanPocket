#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Umeshu.Utility
{
    [CustomPropertyDrawer(typeof(Distribution<>), true)]
    public class DistributionEditor : PropertyDrawer
    {
        private const string CURSORS_NAME = "maxCursors", ASSOCIATION_NAME = "association";
        protected static float SingleLineHeight => EditorGUIUtility.singleLineHeight;
        protected static float HeaderHeight => SingleLineHeight;
        private SerializedProperty property, cursors, associations;
        private float[] values;

        public override float GetPropertyHeight(SerializedProperty _property, GUIContent _label)
        {
            if (!_property.isExpanded)
                return HeaderHeight;
            SerializedProperty _association = _property.FindPropertyRelative(ASSOCIATION_NAME);
            int _arraySize = _association.arraySize;
            if (_arraySize == 0)
                return HeaderHeight;
            float _size = HeaderHeight;
            float _elementSize = MinMaxRangeEditor.GetPropertyHeight(MultiPropertyDrawMode.values);
            for (int _i = 0; _i < _arraySize; _i++)
            {
                _size += _elementSize + EditorGUI.GetPropertyHeight(_association.GetArrayElementAtIndex(_i));
            }
            return _size;
        }


        public override bool CanCacheInspectorGUI(SerializedProperty _property) => false;
        public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
        {
            OpenBackEnd(_property);
            DoFrontEnd(_position);
            CloseBackEnd();
        }

        #region BackEnd

        private void OpenBackEnd(SerializedProperty _property)
        {
            InitBackendVars(_property);
            FixArrays();
        }
        void InitBackendVars(SerializedProperty _property)
        {
            this.property = _property;
            cursors = _property.FindPropertyRelative(CURSORS_NAME);
            associations = _property.FindPropertyRelative(ASSOCIATION_NAME);
            values = GetValues();
        }

        private float[] GetValues()
        {
            float[] _values = new float[cursors.arraySize];
            for (int _i = 0; _i < _values.Length; _i++) _values[_i] = cursors.GetArrayElementAtIndex(_i).floatValue;
            return _values;
        }
        private void FixArrays()
        {
            int _size = associations.arraySize;
            if (cursors.arraySize != _size - 1 && _size != 0)
            {
                cursors.arraySize = _size - 1;
            }
        }

        private void CloseBackEnd()
        {
            Fix();
            for (int _i = 0; _i < cursors.arraySize; _i++) cursors.GetArrayElementAtIndex(_i).floatValue = values[_i];
        }
        void Fix()
        {
            float[] _old = GetValues();
            int _changeIndex = -1;
            for (int _i = 0; _i < _old.Length; _i++) if (_old[_i] != values[_i])
                {
                    _changeIndex = _i;
                    break;
                }
            FixFromIndex(_changeIndex, _old);
        }
        void FixFromIndex(int _index, float[] _old)
        {
            if (_index < 0 || _index >= values.Length) return;
            if (values[_index] == _old[_index]) return;

            if (_index - 1 >= 0)
                if (values[_index - 1] > values[_index])
                {
                    values[_index - 1] = values[_index];
                    FixFromIndex(_index - 1, _old);
                }
            if (_index + 1 < values.Length)
                if (values[_index + 1] < values[_index])
                {
                    values[_index + 1] = values[_index];
                    FixFromIndex(_index + 1, _old);
                }
        }
        #endregion BackEnd

        private void DoFrontEnd(Rect _position)
        {
            Rect _current = new(_position) { height = HeaderHeight };
            property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(_current, property.isExpanded, property.displayName);
            if (!property.isExpanded)
            {
                EditorGUI.EndFoldoutHeaderGroup();
                return;
            }
            EditorGUI.indentLevel++;
            _current = MoveRectDown(_current);
            _current.Set(_current.x + 15, _current.y, _current.width - 15, _current.height);
            DrawElements(_current);
            EditorGUI.indentLevel--;
            EditorGUI.EndFoldoutHeaderGroup();
        }


        private void DrawElements(Rect _current)
        {
            for (int _i = 0; _i < associations.arraySize; _i++)
            {
                _current.height = EditorGUI.GetPropertyHeight(associations.GetArrayElementAtIndex(_i), true);
                EditorGUI.PropertyField(_current, associations.GetArrayElementAtIndex(_i), true);
                _current = MinMaxRangeEditor.GetRect(_current.min + Vector2.up * _current.height, _current.width, MultiPropertyDrawMode.values);
                _current = DrawRangeValueAt(_current, _i);
            }
        }

        private Rect DrawRangeValueAt(Rect _current, int _index)
        {
            float[] _currentValues = GetMinMaxAtIndex(_index);
            float[] _old = new float[2];
            _currentValues.CopyTo(_old, 0);
            MinMaxRangeEditor.DrawMinMaxRange(_current, ref _currentValues, 0, 1, GUIContent.none, MultiPropertyDrawMode.values);
            // if (!EditorGUIUtility.editingTextField)
            // {
            if (_currentValues[0] != _old[0])
                SetMinAtIndex(_index, _currentValues[0]);
            if (_currentValues[1] != _old[0])
                SetMaxAtIndex(_index, _currentValues[1]);
            // }
            return MoveRectDown(_current);
        }

        private float[] GetMinMaxAtIndex(int _index) => new float[] { GetMinAtIndex(_index), GetMaxAtIndex(_index) };
        private float GetMinAtIndex(int _index) => _index > 0 ? cursors.GetArrayElementAtIndex(_index - 1).floatValue : 0;

        private float GetMaxAtIndex(int _index) => _index < cursors.arraySize ? cursors.GetArrayElementAtIndex(_index).floatValue : 1;

        private void SetMinAtIndex(int _index, float _min)
        {
            if (_index > 0)
            {
                values[_index - 1] = _min;
            }
        }
        private void SetMaxAtIndex(int _index, float _max)
        {
            if (_index < values.Length)
            {
                values[_index] = _max;
            }
        }


        static Rect MoveRectDown(Rect _rect) => new(_rect.xMin, _rect.yMax, _rect.width, EditorGUIUtility.singleLineHeight);
    }
}

#endif