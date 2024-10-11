#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using Umeshu.Uf;
using Umeshu.Utility;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using PopupWindow = UnityEditor.PopupWindow;

namespace Umeshu.Common
{
    [CustomPropertyDrawer(typeof(UAsset<>), true)]
    public class UAssetUIE : PropertyDrawerUtil
    {
        private SerializedProperty PackageReferenceProperty => GetProperty(nameof(UAsset<Object>.packageReference));
        private SerializedProperty AssetNameProperty => GetProperty(nameof(UAsset<Object>.assetName));

        private UAsset_Database dataBase;
        private AssetLabelReference packageReference;

        private const int BUTTON_SIZE = 24;
        private const int OBJECT_SIZE = 20;
        private float positionWidth = 0f;
        private float NonButtonPropertySpace => positionWidth - BUTTON_SIZE - OBJECT_SIZE;
        private bool validLabel = false;

        public override float GetPropertyHeight(SerializedProperty _property, GUIContent _label)
        {
            return EditorGUIUtility.singleLineHeight; // base.GetPropertyHeight(_property, _label);
        }

        public override void OnCustomGUI(ref Rect _position, SerializedProperty _property, GUIContent _label)
        {
            positionWidth = _position.width;
            dataBase = UfEditor.GetAssetOfType<UAsset_Database>();

            if (dataBase == null) DisplayButtonToCreateDatabase(ref _position);
            else DisplayNormalBehaviour(ref _position, _label);
        }

        private void DisplayButtonToCreateDatabase(ref Rect _position)
        {
            if (GUI.Button(_position, new GUIContent("Create Database")))
            {
                UfEditor.CreateScritableObjectInProject<UAsset_Database>();
            }
        }

        private void DisplayNormalBehaviour(ref Rect _position, GUIContent _label)
        {
            packageReference = PackageReferenceProperty.GetUnderlyingValue() as AssetLabelReference;

            DisplayPackageReference(ref _position, _label);
            if (packageReference.RuntimeKeyIsValid() && packageReference.labelString != "default")
            {
                DisplayAssetName(ref _position);
                DisplayAddButton(ref _position);
                DisplayObjectField(ref _position);
            }

        }

        #region Property display

        private void DisplayPackageReference(ref Rect _position, GUIContent _label)
        {
            _position.width = NonButtonPropertySpace * .72f;
            if (!property.IsArrayElement()) AddGenericTypeStringToLabel(_label);
            EditorGUI.PropertyField(_position, PackageReferenceProperty, _label);
        }

        private void DisplayAssetName(ref Rect _position)
        {
            _position.x += _position.width;
            _position.width = NonButtonPropertySpace - _position.width;

            DatabaseEntry _entry = dataBase.entries.Find(_entry => _entry.group.labelString == packageReference.labelString);
            bool _entryIsInDatabase = _entry != null;
            validLabel = false;
            if (_entryIsInDatabase)
            {
                string[] _possibleEntries = dataBase.entries.ExtractArray(_entry => _entry.key, _entry => _entry.group.labelString == packageReference.labelString);
                if (string.IsNullOrEmpty(AssetNameProperty.stringValue) && _possibleEntries.Length > 0) AssetNameProperty.stringValue = _possibleEntries[0];

                int _choiceIndex = _possibleEntries.IndexOf(AssetNameProperty.stringValue);
                if (_choiceIndex > -1)
                {
                    _choiceIndex = EditorGUI.Popup(_position, _choiceIndex, _possibleEntries.ToArray());
                    AssetNameProperty.stringValue = _possibleEntries[_choiceIndex];
                    validLabel = true;
                }
                else DisplayTextField(_position);
            }
        }

        private void DisplayTextField(Rect _position)
        {
            GUIStyle _style = new("SearchTextField");
            _style.normal.textColor = Color.red;
            AssetNameProperty.stringValue = EditorGUI.TextField(_position, AssetNameProperty.stringValue, _style);
        }

        private void DisplayObjectField(ref Rect _position)
        {
            _position.x += _position.width;
            _position.width = OBJECT_SIZE;

            Object _newObject = EditorGUI.ObjectField(_position, "", null, typeof(Object), false) as Object;

            if (_newObject != null)
            {
                AssetNameProperty.stringValue = _newObject.name;
            }
        }

        private void DisplayAddButton(ref Rect _position)
        {
            int _indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            _position.x += _position.width + BUTTON_SIZE;
            _position.width = BUTTON_SIZE;
            _position.height = EditorGUIUtility.singleLineHeight;
            _position.x -= _position.width;
            if (GUI.Button(_position, new GUIContent("+"))) OnAddLabel(_position);
            EditorGUI.indentLevel = _indent;
        }

        private void OnAddLabel(Rect _buttonRect)
        {
            _buttonRect.x = 6;
            _buttonRect.y += 5;
            PopupWindow.Show(_buttonRect, new LabelNamePopup(positionWidth, EditorGUIUtility.singleLineHeight * 1.5f, dataBase, packageReference, validLabel ? "" : AssetNameProperty.stringValue));
        }

        #endregion

    }


    public class LabelNamePopup : PopupWindowContent
    {
        public float windowWidth;
        public float rowHeight;
        public string name;
        public bool needsFocus = true;
        public UAsset_Database settings;
        public List<DatabaseEntry> existingEntries;
        public AssetLabelReference assetLabelReference;

        public LabelNamePopup(float _width, float _rowHeight, UAsset_Database _settings, AssetLabelReference _assetLabelReference, string _invalidEntry)
        {
            this.windowWidth = _width;
            this.rowHeight = _rowHeight;
            this.settings = _settings;
            this.assetLabelReference = _assetLabelReference;
            this.existingEntries = _settings.GetEntries(_assetLabelReference);
            name = _invalidEntry;
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(windowWidth - 13f, rowHeight * 2.25f);
        }

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
                if (string.IsNullOrEmpty(name)) Debug.LogError("Cannot add empty label to Addressables label list");
                else if (existingEntries.Find(_entry => _entry.key == name) != null) Debug.LogError("Label name '" + name + "' is already in the labels list.");
                else
                {
                    settings.entries.Add(new(assetLabelReference, name));
                    UfEditor.AddObjectToDirtyAndSaveIt(settings);
                }
                editorWindow.Close();
            }
        }
    }
}
#endif