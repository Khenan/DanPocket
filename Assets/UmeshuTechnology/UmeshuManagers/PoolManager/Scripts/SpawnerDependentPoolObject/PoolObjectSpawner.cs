using System;
using System.Collections.Generic;
using Umeshu.Common;
using Umeshu.USystem;
using UnityEngine;

public abstract class PoolObjectSpawner<TObj, TStruct> : HeritableGameElement, IPoolObjectSpawner where TObj : SpawnerDependentPoolObject<TStruct> where TStruct : struct
{
    [SerializeField] private UPoolableAsset<TObj> poolObjectToSpawn;
    [SerializeField] protected TStruct objectData;
    [SerializeField] private bool spawnAtLevelInstantiation = true;
    [HideInInspector] public TObj lastInstantiatedObject;

    public bool SpawnAtLevelInstantiation => spawnAtLevelInstantiation;

    public void SpawnObject()
    {
        lastInstantiatedObject = this.GetChildFromPool(false, poolObjectToSpawn);
        lastInstantiatedObject.transform.SetPositionAndRotation(transform.position, transform.rotation);
        lastInstantiatedObject.transform.localScale = transform.localScale;

        OnObjectSpawned(lastInstantiatedObject);
    }
    public T GetStructData<T>() where T : struct => (T)Convert.ChangeType(objectData, typeof(T));
    protected abstract void OnObjectSpawned(TObj _spawnedObject);

    #region Game Element Methods

    protected sealed override void GameElementFirstInitialize() { }
    protected sealed override void GameElementEnableAndReset() { }
    protected sealed override void GameElementPlay() { }
    protected sealed override void GameElementUpdate() { }

    #endregion
}

public interface IPoolObjectSpawner
{
    void SpawnObject();
    T GetStructData<T>() where T : struct;
    public bool SpawnAtLevelInstantiation { get; }
}