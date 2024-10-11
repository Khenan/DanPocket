using System.Collections;
using System.Collections.Generic;
using Umeshu.Uf;
using UnityEngine.AddressableAssets;

namespace Umeshu.Common
{
    [System.Serializable]
    public class UAssetCollection<T> : IUAssetDepedency, IList<UAsset<T>> where T : UnityEngine.Object
    {
        public List<AssetLabelReference> PackageReferences => uAssets.ExtractList(_uAsset => _uAsset.packageReference);
        public List<UAsset<T>> uAssets = new();

        public static implicit operator List<UAsset<T>>(UAssetCollection<T> _collection) => _collection.uAssets;

        #region IList
        public int Count => ((ICollection<UAsset<T>>)uAssets).Count;

        public bool IsReadOnly => ((ICollection<UAsset<T>>)uAssets).IsReadOnly;

        public UAsset<T> this[int _index] { get => ((IList<UAsset<T>>)uAssets)[_index]; set => ((IList<UAsset<T>>)uAssets)[_index] = value; }

        public int IndexOf(UAsset<T> _item) => ((IList<UAsset<T>>)uAssets).IndexOf(_item);

        public void Insert(int _index, UAsset<T> _item) => ((IList<UAsset<T>>)uAssets).Insert(_index, _item);

        public void RemoveAt(int _index) => ((IList<UAsset<T>>)uAssets).RemoveAt(_index);

        public void Add(UAsset<T> _item) => ((ICollection<UAsset<T>>)uAssets).Add(_item);

        public void Clear() => ((ICollection<UAsset<T>>)uAssets).Clear();

        public bool Contains(UAsset<T> _item) => ((ICollection<UAsset<T>>)uAssets).Contains(_item);

        public void CopyTo(UAsset<T>[] _array, int _arrayIndex) => ((ICollection<UAsset<T>>)uAssets).CopyTo(_array, _arrayIndex);

        public bool Remove(UAsset<T> _item) => ((ICollection<UAsset<T>>)uAssets).Remove(_item);

        public IEnumerator<UAsset<T>> GetEnumerator() => ((IEnumerable<UAsset<T>>)uAssets).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)uAssets).GetEnumerator();
        #endregion
    }

}