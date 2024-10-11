using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Umeshu.Common;
using Umeshu.Uf;
using Umeshu.USystem;
using Umeshu.USystem.GameData;
using Umeshu.Utility;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

[RequireComponent(typeof(UIFadedRoot))]
public class CameraScrollUIFadedRoot : MonoBehaviour, IUIFadedRootTrigger
{
    private CameraScrollRuntimeData cameraScrollRunTimeData;
    public bool UIIsDisplayed => GetData() == null || cameraScrollRunTimeData.showUI;

    private CameraScrollRuntimeData GetData()
    {
        cameraScrollRunTimeData = GameDataManager.GetData<CameraScrollRuntimeData>();
        return cameraScrollRunTimeData;
    }
}