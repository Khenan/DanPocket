#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

public static class EditorGUIUtilityExtension
{
    public static void RepaintInspectors()
    {
        Editor[] _ed = Resources.FindObjectsOfTypeAll<Editor>();
        for (int _i = 0; _i < _ed.Length; _i++)
        {
            Debug.Log(_i);
            _ed[_i].ReloadPreviewInstances();
            _ed[_i].OnInspectorGUI();
        }
    }
    public static float IndentWidth => EditorGUI.indentLevel * 15f;
    public static float LabelOffset => EditorGUIUtility.labelWidth - IndentWidth;
    public static Vector2 LabelOffsetVector => Vector2.right * LabelOffset;
    public static Rect GetFieldRectOf(this Rect _position) => new(_position.position + LabelOffsetVector, new(_position.width - LabelOffset, EditorGUIUtility.singleLineHeight));
}

#endif