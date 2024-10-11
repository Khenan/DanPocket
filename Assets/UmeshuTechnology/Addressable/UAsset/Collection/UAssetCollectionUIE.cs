#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Umeshu.Uf;
using Umeshu.Utility;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using PopupWindow = UnityEditor.PopupWindow;

namespace Umeshu.Common
{
    [CustomPropertyDrawer(typeof(UAssetCollection<>), true)]
    public class UAssetCollectionUIE : PropertyDrawerUtil
    {
        private SerializedProperty UAssetProperty => GetProperty(nameof(UAssetCollection<Object>.uAssets));

        public override void OnCustomGUI(ref Rect _position, SerializedProperty _property, GUIContent _label)
        {
            AddGenericTypeStringToLabel(_label);
            EditorGUI.PropertyField(_position, UAssetProperty, _label);
        }

        public override float GetPropertyHeight(SerializedProperty _property, GUIContent _label)
        {
            return EditorGUI.GetPropertyHeight(_property.FindPropertyRelative(nameof(UAssetCollection<Object>.uAssets)), _label);
        }
    }
}
#endif