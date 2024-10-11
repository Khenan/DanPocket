using Umeshu.USystem;
using Umeshu.USystem.Pool;
using UnityEngine;

namespace Umeshu.Common
{
    [System.Serializable]
    public class UPoolableAsset<T> : UComponentAsset<T>, IUPoolableAsset where T : PoolableGameElement
    {
        public IGameElement GetComponentAsset() => Resources.Load<T>(assetName);
        public T GetAssetFromPool(IGameElement _parent)
        {
            T _asset = Resources.Load<T>(assetName);
            return _parent.GetObject(_asset);
        }
    }
}