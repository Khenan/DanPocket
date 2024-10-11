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
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

[CustomEditor(typeof(AutoSpriteLink<,,>), true)]
public class AutoSpriteLinkEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // button to update sprite
        if (GUILayout.Button("Update Sprite"))
        {
            if (target is IAutoSpriteLink _autoSpriteLink)
            {
                Component _component = _autoSpriteLink.UpdateSprite();
                if (_component == null) return;
                // set dirty
                EditorUtility.SetDirty(target);
                EditorUtility.SetDirty(_component);
            }
        }
    }
}



#endif