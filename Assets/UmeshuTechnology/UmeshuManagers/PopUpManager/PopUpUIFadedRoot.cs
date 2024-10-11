using System;
using System.Collections;
using System.Collections.Generic;
using Umeshu.Common;
using Umeshu.Uf;
using Umeshu.USystem;
using Umeshu.USystem.GameData;
using Umeshu.USystem.PopUp;
using Umeshu.Utility;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

[RequireComponent(typeof(UIFadedRoot))]
public class PopUpUIFadedRoot : MonoBehaviour, IUIFadedRootTrigger
{
    private PopUpRuntimeData runTimeData;
    public bool UIIsDisplayed => GetData() == null || !runTimeData.uiShouldBeHidden;

    private PopUpRuntimeData GetData()
    {
        runTimeData = GameDataManager.GetData<PopUpRuntimeData>();
        return runTimeData;
    }
}
