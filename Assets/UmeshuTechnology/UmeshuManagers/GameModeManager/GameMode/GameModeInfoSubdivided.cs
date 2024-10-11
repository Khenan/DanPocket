using System;
using System.Collections;
using System.Collections.Generic;
using Umeshu.Common;
using Umeshu.Uf;
using Umeshu.USystem;
using Umeshu.Utility;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public abstract class GameModeInfoSubdivided<T> : GameModeInfo where T : Enum
{
    [Header("Subdivided Datas")]
    [SerializeField] private EnumBasedSelector<T, GameModeLoadInfos> subdividedGameModeLoadInfo = new();
    public abstract T GetSubdividedPackage();

    private GameModeLoadInfos AccurateGameModeLoadInfos => subdividedGameModeLoadInfo.GetValue(GetSubdividedPackage());
    public override string[] GetScenes() => base.GetScenes().GetMergedWith(AccurateGameModeLoadInfos.Scenes);
    public override AssetLabelReference[] GetPackages() => base.GetPackages().GetMergedWith(AccurateGameModeLoadInfos.packages);
    public override UPoolableAsset<PoolableGameElement>[] GetPoolableGameElements() => base.GetPoolableGameElements().GetMergedWith(AccurateGameModeLoadInfos.poolableGameElements);
}
