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
    [CustomPropertyDrawer(typeof(DatabaseEntry), true)]
    public class DatabaseEntryUIE : PropertyDrawerUtil
    {
        private SerializedProperty GroupProperty => GetProperty(nameof(DatabaseEntry.group));
        private SerializedProperty KeyProperty => GetProperty(nameof(DatabaseEntry.key));

        public override void OnCustomGUI(ref Rect _position, SerializedProperty _property, GUIContent _label)
        {
            List<SerializedProperty> _properties = new() { GroupProperty, KeyProperty };
            _position.width /= (float)_properties.Count;
            foreach (SerializedProperty _localProperty in _properties)
            {
                EditorGUI.PropertyField(_position, _localProperty, GUIContent.none, true);
                _position.x += _position.width;
            }
        }
    }
}
#endif