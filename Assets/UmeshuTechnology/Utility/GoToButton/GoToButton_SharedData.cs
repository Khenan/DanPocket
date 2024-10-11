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

[CreateAssetMenu(fileName = "GoToButton_SharedData_", menuName = "ScriptableObjects/Graine/GoToButton_SharedData")]
public class GoToButton_SharedData : ScriptableObject
{
    [Header("Highlight")]
    public AnimationCurve highlightCurveX = AnimationCurve.Linear(0, 0, 1, 1);
    public AnimationCurve highlightCurveY = AnimationCurve.Linear(0, 0, 1, 1);
    public float highlightFrequency = 2f;
    public float highlightScaleMagnitude = .5f;
}
