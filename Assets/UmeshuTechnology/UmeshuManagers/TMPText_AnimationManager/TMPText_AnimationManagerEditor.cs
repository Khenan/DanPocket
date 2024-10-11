#if UNITY_EDITOR

using Umeshu.USystem.TextAnimation;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TMPText_AnimationManager))]
public class TMPText_AnimationManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Fill Prebuild Animation Slots"))
            (target as TMPText_AnimationManager).FillPrebuildAnimationSlots();
    }
}

#endif