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

public abstract class HeritableGameElement : GameElement
{

    protected sealed override bool Pausable => true;

    public sealed override void InitBranch() => base.InitBranch();
    protected sealed override void Update() => base.Update();
}