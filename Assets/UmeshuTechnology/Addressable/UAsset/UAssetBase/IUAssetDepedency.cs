using System.Collections.Generic;
using UnityEngine.AddressableAssets;

public interface IUAssetDepedency
{
    public List<AssetLabelReference> PackageReferences { get; }
}
