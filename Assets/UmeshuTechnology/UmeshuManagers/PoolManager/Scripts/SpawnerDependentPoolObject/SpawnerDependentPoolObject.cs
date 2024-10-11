using System;
using System.Collections;
using System.Collections.Generic;
using Umeshu.Common;
using Umeshu.Uf;
using Umeshu.USystem;
using Umeshu.Utility;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public abstract class SpawnerDependentPoolObject<T> : PoolableGameElement where T : struct
{
    private T data;
    public T Data => data;
    internal void SetData(T _objectData) => data = _objectData;
    public abstract T GetDataToSendToSpawner();

    protected internal sealed override void PreInitBranchMethod()
    {
        base.PreInitBranchMethod();
        if (Hierarchy.Parent.Value is IPoolObjectSpawner _spawner)
        {
            SetData(_spawner.GetStructData<T>());

        }
    }
}