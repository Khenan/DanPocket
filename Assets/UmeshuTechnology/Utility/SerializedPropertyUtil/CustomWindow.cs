#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using Umeshu.Common;
using Umeshu.Uf;
using Umeshu.USystem;
using Umeshu.Utility;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Umeshu.Utility
{
    public class CustomWindow : EditorWindow
    {
        private Object modifiedObject;

        public static void CreateWindow(Object _object)
        {
            _object.Log();
            CustomWindow _wnd = GetWindow<CustomWindow>();
            _wnd.titleContent = new GUIContent(_object.name);
            _wnd.modifiedObject = _object;
        }

        private void OnGUI()
        {
            SerializedPropertyUtil.DoInspectorLayout(modifiedObject);
        }
    }
}

#endif