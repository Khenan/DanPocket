using Umeshu.Uf;
using Umeshu.USystem.Addressable;
using UnityEngine;

namespace Umeshu.Common
{
    [System.Serializable]
    public class UComponentAsset<T> : UAsset<T> where T : Component
    {
        public static implicit operator T(UComponentAsset<T> _uAsset) => _uAsset.Value;
        protected override T GetAsset(bool _logErrors = true)
        {
            GameObject _asset = AddressableManager.Instance.GetAsset<GameObject>(packageReference, assetName, typeof(GameObject), _logErrors);
            if (_asset == null && _logErrors) $"Asset {assetName} not found in package {packageReference.labelString}".LogError();
            return _asset != null ? _asset.GetComponent<T>() : null;
        }
    }
}