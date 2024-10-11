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

[CreateAssetMenu(fileName = "LogCategoryData_", menuName = "ScriptableObjects/UmeshuTechnology/LogCategoryData")]
public class LogCategoryData : ScriptableObject
{
    public bool activateFilters = true;
    public List<OptionalVar<string>> logCategories = new();

    internal void AddCategoriesToLogger()
    {
        if (!activateFilters) return;
        foreach (OptionalVar<string> _logCategory in logCategories)
            UfLogger.AddCategory(_logCategory.Enabled, _logCategory.Value);

    }
}