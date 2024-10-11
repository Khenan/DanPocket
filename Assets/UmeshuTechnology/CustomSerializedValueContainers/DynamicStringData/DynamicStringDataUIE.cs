#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Umeshu.Common;
using Umeshu.Uf;
using Umeshu.USystem;
using Umeshu.Utility;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Umeshu.Common
{
    [CustomPropertyDrawer(typeof(DynamicStringData<>), true)]
    public class DynamicStringDataUIE : PropertyDrawerUtil
    {
        private const int BUTTON_SIZE = 24;
        private const int OBJECT_SIZE = 20;
        private float positionWidth = 0f;
        private bool validLabel = false;

        private SerializedProperty ValueProperty => GetProperty(nameof(DynamicStringData<DynamicStringDatabase>.value));


        private static DynamicStringDatabase dataBase;
        public override void OnCustomGUI(ref Rect _position, SerializedProperty _property, GUIContent _label)
        {
            positionWidth = _position.width;
            Type _type = _property.GetGenericArgumentType(0);

            dataBase ??= UfEditor.GetAssetOfType(_type) as DynamicStringDatabase;

            _label.text += " <" + _type.Name + ">";

            if (dataBase == null) DisplayButtonToCreateDatabase(ref _position, _type);
            else DisplayNormalBehaviour(ref _position, _label);
        }

        private void DisplayButtonToCreateDatabase(ref Rect _position, Type _type)
        {
            if (GUI.Button(_position, new GUIContent("Create Database")))
                UfEditor.CreateScritableObjectInProject(_type);
        }
        private void DisplayNormalBehaviour(ref Rect _position, GUIContent _label)
        {
            if (dataBase.entries.Count == 0)
            {
                dataBase.entries.Add("DefaultEntry");
                UfEditor.AddObjectToDirtyAndSaveIt(dataBase);
            }
            DisplayString(ref _position, _label);
            DisplayRemoveButton(ref _position);
            DisplayAddButton(ref _position);
            DisplayOpenDataBaseButton(ref _position);
            DisplayObjectField(ref _position);

        }
        #region Property display

        private void DisplayString(ref Rect _position, GUIContent _label)
        {
            //_position.x += _position.width;
            _position.width = _position.width - BUTTON_SIZE * 3 - OBJECT_SIZE;

            validLabel = false;
            List<string> _possibleEntries = dataBase.entries;
            if (string.IsNullOrEmpty(ValueProperty.stringValue) && _possibleEntries.Count > 0) ValueProperty.stringValue = _possibleEntries[0];

            int _choiceIndex = _possibleEntries.IndexOf(ValueProperty.stringValue);
            if (_choiceIndex > -1)
            {
                _choiceIndex = EditorGUI.Popup(_position, _label.text, _choiceIndex, _possibleEntries.ToArray());
                ValueProperty.stringValue = _possibleEntries[_choiceIndex];
                validLabel = true;
            }
            else DisplayTextField(_position, _label);
        }

        private void DisplayTextField(Rect _position, GUIContent _label)
        {
            GUIStyle _style = new("SearchTextField");
            _style.normal.textColor = Color.red;
            ValueProperty.stringValue = EditorGUI.TextField(_position, _label, ValueProperty.stringValue, _style);
        }

        private void DisplayObjectField(ref Rect _position)
        {
            _position.x += _position.width;
            _position.width = OBJECT_SIZE;

            Object _newObject = EditorGUI.ObjectField(_position, "", null, typeof(Object), false);

            if (_newObject != null) ValueProperty.stringValue = _newObject.name;
        }

        private void HandleButtonPosition(ref Rect _position)
        {
            EditorGUI.indentLevel = 0;
            _position.x += _position.width + BUTTON_SIZE;
            _position.width = BUTTON_SIZE;
            _position.height = EditorGUIUtility.singleLineHeight;
            _position.x -= _position.width;
        }

        private void DisplayRemoveButton(ref Rect _position)
        {
            int _indent = EditorGUI.indentLevel;
            HandleButtonPosition(ref _position);
            if (GUI.Button(_position, new GUIContent("-"))) OnRemoveLabel(ValueProperty.stringValue);
            EditorGUI.indentLevel = _indent;
        }

        private void DisplayAddButton(ref Rect _position)
        {
            int _indent = EditorGUI.indentLevel;
            HandleButtonPosition(ref _position);
            if (GUI.Button(_position, new GUIContent("+"))) OnAddLabel(_position);
            EditorGUI.indentLevel = _indent;
        }

        private void DisplayOpenDataBaseButton(ref Rect _position)
        {
            int _indent = EditorGUI.indentLevel;
            HandleButtonPosition(ref _position);
            if (GUI.Button(_position, new GUIContent("..."))) AssetDatabase.OpenAsset(dataBase);
            EditorGUI.indentLevel = _indent;
        }

        private void OnRemoveLabel(string _label)
        {
            if (UfEditor.CreateConfirmWindow("Label Remove Request", $"Are you sure you want to remove this label : \"{_label}\"?"))
            {
                dataBase.entries.Remove(_label);
                UfEditor.AddObjectToDirtyAndSaveIt(dataBase);
            }
        }

        private void OnAddLabel(Rect _buttonRect)
        {
            _buttonRect.x = 6;
            _buttonRect.y += 5;
            PopupWindow.Show(_buttonRect, new DynamicStringDataPopup(positionWidth, EditorGUIUtility.singleLineHeight * 1.5f, dataBase, validLabel ? "" : ValueProperty.stringValue, ValueProperty));
        }

        #endregion

    }
    public class DynamicStringDataPopup : PopupWindowContent
    {
        public float windowWidth;
        public float rowHeight;
        public string name;
        public bool needsFocus = true;
        public DynamicStringDatabase database;
        public SerializedProperty property;

        public DynamicStringDataPopup(float _width, float _rowHeight, DynamicStringDatabase _database, string _invalidEntry, SerializedProperty _property)
        {
            this.windowWidth = _width;
            this.rowHeight = _rowHeight;
            this.database = _database;
            this.property = _property;
            name = _invalidEntry;
        }

        public override Vector2 GetWindowSize() => new(windowWidth - 13f, rowHeight * 2.25f);

        public override void OnGUI(Rect _windowRect)
        {
            GUILayout.Space(5);
            Event _evt = Event.current;
            bool _hitEnter = _evt.type == EventType.KeyDown && (_evt.keyCode == KeyCode.Return || _evt.keyCode == KeyCode.KeypadEnter);
            UnityEngine.GUI.SetNextControlName("LabelName");
            EditorGUIUtility.labelWidth = 80;
            name = EditorGUILayout.TextField("Label Name", name);
            if (needsFocus)
            {
                needsFocus = false;
                EditorGUI.FocusTextInControl("LabelName");
            }

            UnityEngine.GUI.enabled = name.Length != 0;
            if (GUILayout.Button("Save") || _hitEnter)
            {
                if (string.IsNullOrEmpty(name)) Debug.LogError("Cannot add empty string");
                else if (database.entries.Find(_entry => _entry == name) != null)
                {
                    Debug.Log("String '" + name + "' is already in the list.");
                    property.stringValue = name;
                    property.serializedObject.ApplyModifiedProperties();
                }
                else
                {
                    database.entries.Add(name);
                    database.entries.Sort();
                    property.stringValue = name;
                    property.serializedObject.ApplyModifiedProperties();
                    UfEditor.AddObjectToDirtyAndSaveIt(database);
                }
                editorWindow.Close();
            }
        }
    }
}

#endif