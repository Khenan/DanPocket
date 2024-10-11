using System;
using System.Collections;
using System.Collections.Generic;
using Umeshu.Common;
using Umeshu.Uf;
using Umeshu.USystem;
using Umeshu.USystem.Pool;
using Umeshu.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class GameModeSceneProxy : PoolableGameElement
{

    #region Game Element Methods

    protected override void GameElementFirstInitialize() { }
    protected override void GameElementEnableAndReset() { }
    protected override void GameElementPlay() { }
    protected override void GameElementUpdate() { }

    #endregion
    
    public void LeaveScene()
    {
        List<IGameElement> _subElementsList = new(Hierarchy.GetSubElements());
        RemoveAllNodeChildrens();
        transform.MoveChildrens(null);
        Stop();
        foreach (IGameElement _subElement in _subElementsList)
            _subElement.Stop();
    }
    
    public void SetAsSceneParent(Scene _scene)
    {
        name = _scene.name + "_RootProxy";
        transform.SetAsRootOfScene(_scene);

        foreach (Transform _child in transform)
            if (_child.TryGetComponent(out IGameElement _gameElement))
                _gameElement.SetGameElementParent(this);
    }

}