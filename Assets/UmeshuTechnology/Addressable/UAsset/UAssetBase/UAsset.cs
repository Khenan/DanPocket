using System.Collections.Generic;
using Umeshu.USystem.Addressable;
using UnityEngine.AddressableAssets;

namespace Umeshu.Common
{
    [System.Serializable]
    public class UAsset<T> : IUAssetDepedency where T : UnityEngine.Object
    {
        public List<AssetLabelReference> PackageReferences => new() { packageReference };
        public AssetLabelReference packageReference = new();
        public string assetName = "";

        public int Key => packageReference.GetHashCode();

        private T value;
        public T Value
        {
            get
            {
                if (value == null) value = GetAsset();
                return value;
            }
        }

        public static implicit operator T(UAsset<T> _uAsset) => _uAsset.Value;
        protected virtual T GetAsset(bool _logErrors = true) => AddressableManager.Instance.GetAsset<T>(packageReference, assetName);
        public bool AssetExists() => GetAsset(false) != null;
    }

}