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
    [CustomPropertyDrawer(typeof(PickableString<>), true)]
    public class PickableStringUIE : PropertyDrawerUtil
    {
        private SerializedProperty ValueProperty => GetProperty(nameof(PickableString<DummyPickableStringDatabase>.value));

        public override void OnCustomGUI(ref Rect _position, SerializedProperty _property, GUIContent _label)
        {
            Type _type = _property.GetGenericArgumentType(0);
            IPickableStringDatabase _dataBase = UfEditor.GetAssetOfType(_type) as IPickableStringDatabase;
            string[] _collection = _dataBase.GetCollection();
            if (string.IsNullOrEmpty(ValueProperty.stringValue) && _collection.Length > 0) ValueProperty.stringValue = _collection[0];

            int _choiceIndex = _collection.IndexOf(ValueProperty.stringValue);
            if (_choiceIndex > -1)
            {
                _choiceIndex = EditorGUI.Popup(_position, _label.text, _choiceIndex, _collection.ToArray());
                ValueProperty.stringValue = _collection[_choiceIndex];
            }
            else
            {
                GUIStyle _style = new("SearchTextField");
                _style.normal.textColor = Color.red;
                ValueProperty.stringValue = EditorGUI.TextField(_position, _label, ValueProperty.stringValue, _style);
            }
        }
    }
}
#endif