#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Umeshu.USystem.MaterialManager
{
    [CustomEditor(typeof(MaterialManager))]
    public class MaterialManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            MaterialManager _shaderHandler = target as MaterialManager;
            if (_shaderHandler.shaderLinks == null || _shaderHandler.shaderLinks.Count == 0) return;
            if (GUILayout.Button("Get all materials")) GetMaterials();
        }

        private void GetMaterials()
        {
            MaterialManager _shaderHandler = target as MaterialManager;

            string[] _allMaterials = AssetDatabase.FindAssets("t:Material");
            Material[] _allMaterialsInProject = new Material[_allMaterials.Length];

            for (int _i = 0; _i < _allMaterials.Length; _i++)
            {
                _allMaterials[_i] = AssetDatabase.GUIDToAssetPath(_allMaterials[_i]);
                _allMaterialsInProject[_i] = AssetDatabase.LoadAssetAtPath<Material>(_allMaterials[_i]);
            }

            foreach (MaterialManager.ShaderLink _shaderLink in _shaderHandler.shaderLinks)
            {
                List<string> _materialNames = new();
                foreach (Shader _shader in _shaderLink.shaders)
                {
                    foreach (Material _material in _allMaterialsInProject)
                    {
                        if (_material.shader != _shader || _materialNames.Contains(_material.name)) continue;
                        _materialNames.Add(_material.name);
                    }
                }
                _shaderLink.SetMaterialNames(_materialNames);
            }

            EditorUtility.SetDirty(target);
        }
    }
}

#endif
