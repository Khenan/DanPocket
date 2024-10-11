using System;
using System.Collections.Generic;
using Umeshu.Common;
using Umeshu.Uf;
using Umeshu.USystem;
using Umeshu.USystem.Addressable;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Umeshu.USystem.MaterialManager
{
    public class MaterialManager : GameSystem<MaterialManager>
    {
        [Header("Links")]
        public AssetLabelReference assetLabelReference;
        public List<ShaderLink> shaderLinks;

        [System.Serializable]
        public class ShaderLink
        {
            public string key;
            public List<Shader> shaders;
            [HideInInspector] public List<Material> materialsFromNames;
            public List<string> materialNames;
            public void SetMaterialNames(List<string> _materialNames) => materialNames = _materialNames;
        }

        private static List<ShaderLink> ShaderLinks { get; set; }

        private void UpdateMaterialsFromNames()
        {
            foreach (ShaderLink _link in shaderLinks)
            {
                _link.materialsFromNames.Clear();
                foreach (string _materialName in _link.materialNames)
                {
                    Material _material = AddressableManager.Instance.GetAsset<Material>(assetLabelReference, _materialName, _logErrors: false);
                    if (_material == null) continue;
                    _link.materialsFromNames.Add(_material);
                }
            }
        }

        private static List<Material> GetMaterials(params string[] _keys)
        {
            if (_keys == null || _keys.Length == 0 || ShaderLinks == null) return new();
            List<Material> _allMaterials = new();
            foreach (string _key in _keys)
            {
                ShaderLink _link = ShaderLinks.Find(_x => _x.key == _key);
                if (_link == null) continue;
                List<Material> _shaderMats = _link.materialsFromNames;
                if (_shaderMats == null || _shaderMats.Count == 0) continue;
                _allMaterials = UfCollection.MergeList(_allMaterials, _shaderMats);
            }
            return _allMaterials;
        }

        public static readonly Dictionary<string, StockInitialValue<Color>> colorChanges = new();
        public static readonly Dictionary<string, StockInitialValue<float>> floatChanges = new();
        public static readonly Dictionary<string, StockInitialValue<int>> intChanges = new();
        public static readonly Dictionary<string, StockInitialValue<Texture>> texChanges = new();
        public static readonly Dictionary<string, StockInitialValue<Vector4>> vectorChanges = new();

        public static void SetColor(string _key, string _shaderKey, Color _value) { foreach (Material _mat in GetMaterials(_key)) { DoChange(_mat, _shaderKey, _value, _mat.GetColor, _mat.SetColor, colorChanges); } }
        public static void SetFloat(string _key, string _shaderKey, float _value) { foreach (Material _mat in GetMaterials(_key)) { DoChange(_mat, _shaderKey, _value, _mat.GetFloat, _mat.SetFloat, floatChanges); } }
        public static void SetInt(string _key, string _shaderKey, int _value) { foreach (Material _mat in GetMaterials(_key)) { DoChange(_mat, _shaderKey, _value, _mat.GetInt, _mat.SetInt, intChanges); } }
        public static void SetTexture(string _key, string _shaderKey, Texture _value) { foreach (Material _mat in GetMaterials(_key)) { DoChange(_mat, _shaderKey, _value, _mat.GetTexture, _mat.SetTexture, texChanges); } }
        public static void SetVector(string _key, string _shaderKey, Vector4 _value) { foreach (Material _mat in GetMaterials(_key)) { DoChange(_mat, _shaderKey, _value, _mat.GetVector, _mat.SetVector, vectorChanges); } }

        private static void DoChange<T>(Material _mat, string _shaderKey, T _value, Func<string, T> _funcToGetValue, Action<string, T> _actionToSetValue, Dictionary<string, StockInitialValue<T>> _dicOfChanges)
        {
#if UNITY_EDITOR
            string _dicKey = _mat.GetInstanceID() + "/" + _shaderKey;
            if (!_dicOfChanges.ContainsKey(_dicKey))
            {
                _dicOfChanges.Add(_dicKey, new StockInitialValue<T>(_shaderKey, _funcToGetValue.Invoke(_shaderKey), _actionToSetValue));
            }
#endif
            _actionToSetValue.Invoke(_shaderKey, _value);
        }

        private void OnDisable()
        {
#if UNITY_EDITOR
            ResetDictionary(colorChanges);
            ResetDictionary(floatChanges);
            ResetDictionary(intChanges);
            ResetDictionary(texChanges);
            ResetDictionary(vectorChanges);

            SetColor("GameLine", "_Color", Color.white);
            SetColor("GameLine", "_RegulatorColor", Color.red);
#endif
        }

        private void ResetDictionary<T>(Dictionary<string, StockInitialValue<T>> _dictionary)
        {
            foreach (StockInitialValue<T> _value in _dictionary.Values)
            {
                _value.ResetValue();
            }
        }

        protected override void SystemFirstInitialize()
        {
            ShaderLinks = shaderLinks;
            AddressableManager.onMainPackageLoad += CheckIfShouldUpdateMaterials;
        }

        private void CheckIfShouldUpdateMaterials(AssetLabelReference _loadedPackage)
        {
            if (_loadedPackage.labelString != assetLabelReference.labelString) return;
            UpdateMaterialsFromNames();
        }

        protected override void SystemEnableAndReset()
        {

        }

        protected override void SystemPlay()
        {

        }

        protected override void SystemUpdate()
        {

        }

        public class StockInitialValue<T>
        {
            public string shaderKey;
            public T initialValue;
            public Action<string, T> actionToCall;

            public StockInitialValue(string _shaderKey, T _initialValue, Action<string, T> _actionToCall)
            {
                shaderKey = _shaderKey;
                initialValue = _initialValue;
                actionToCall = _actionToCall;
            }

            public void ResetValue() => actionToCall.Invoke(shaderKey, initialValue);
        }
    }
}