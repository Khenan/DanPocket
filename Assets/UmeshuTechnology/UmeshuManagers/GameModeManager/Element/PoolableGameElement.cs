using System.Collections.Generic;
using System.Linq;
using Umeshu.Uf;
using Umeshu.USystem;
using Umeshu.USystem.Pool;
using Umeshu.Utility;
using UnityEngine;


public abstract class PoolableGameElement : GameElement
{
    #region Security
    protected sealed override void Awake() { } // Prevent usage in childrens
    protected sealed override void Start() { } // Prevent usage in childrens
    protected sealed override void Reset() { } // Prevent usage in childrens
    #endregion

    [Header("Pool index")]
    [SerializeField] private OptionalVar<int> poolIndex = new(false);

    public int GetPoolIndex() => poolIndex.Enabled ? poolIndex.Value : GetInstanceID();
    public PoolableChildTransferer MethodTransferer { get; private set; }
    public override bool CanReparent => true;
    protected sealed override bool Pausable => true;

    private bool isPoolObjectAvailableFromScript = true;

    public sealed override void InitBranch()
    {
        isPoolObjectAvailableFromScript = false;
        MethodTransferer = GetComponent<PoolableChildTransferer>();
        PreInitBranchMethod();
        base.InitBranch();
    }

    protected sealed override void Update() => base.Update();

    protected internal virtual void PreInitBranchMethod()
    {

    }

    public override bool IsAvailable() => base.IsAvailable() && isPoolObjectAvailableFromScript;

    public override void Stop()
    {
        base.Stop();
        isPoolObjectAvailableFromScript = true;
        PoolManager.GoBackToPool(this);
    }

    #region Security Log

    internal void FlagAsSafeDestroy() => flagedAsValidDestroy = true;
    private static bool applicationIsQuitting = false;
    private bool flagedAsValidDestroy = false;
    private void OnApplicationQuit() => applicationIsQuitting = true;
    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (applicationIsQuitting || flagedAsValidDestroy) return;
        ("Destroying " + gameObject.GetHierarchyPath().Bold().Color(Color.red) + " while it shouldnt").LogError();

    }

    #endregion

}