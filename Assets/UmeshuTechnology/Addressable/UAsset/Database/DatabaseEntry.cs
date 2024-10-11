using UnityEngine.AddressableAssets;

namespace Umeshu.Common
{
    [System.Serializable]
    public class DatabaseEntry
    {
        public AssetLabelReference group;
        public string key;

        public DatabaseEntry(AssetLabelReference _group, string _key)
        {
            group = _group;
            key = _key;
        }

        public override string ToString() => group + " / " + key;
    }

}
