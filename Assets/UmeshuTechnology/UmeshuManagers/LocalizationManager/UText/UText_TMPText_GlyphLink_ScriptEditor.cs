#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UText_TMPText_GlyphLink))]
public class UText_TMPText_GlyphLink_ScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        UText_TMPText_GlyphLink _uTextTmpTextGlyphLink = (UText_TMPText_GlyphLink)target;
        if (GUILayout.Button("Center glyphs"))
        {
            foreach (TMPro.TMP_SpriteGlyph _glyph in _uTextTmpTextGlyphLink.spriteAsset.spriteGlyphTable)
            {
                UnityEngine.TextCore.GlyphMetrics _metrics = _glyph.metrics;
                _metrics.horizontalBearingX = 0;
                _metrics.horizontalBearingY = _metrics.height * .75f;
                _metrics.horizontalAdvance = _metrics.width;
                _glyph.metrics = _metrics;
            }
            EditorUtility.SetDirty(_uTextTmpTextGlyphLink.spriteAsset);
        }
    }
}

#endif