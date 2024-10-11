using System;
using System.Collections;
using System.Collections.Generic;
using Umeshu.Common;
using Umeshu.Uf;
using Umeshu.USystem;
using Umeshu.Utility;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class GoToButton_ButtonLink : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image[] images;
    [SerializeField] private Button button;
    [SerializeField] private GoToButton_SharedData data;

    [Header("Parameters")]
    [SerializeField] private OptionalVar<Transform> transformToHighlight = new(null, false);

    public void SetEnabled(bool _value)
    {
        button.interactable = _value;
        foreach (Image _image in images)
            _image.color = _value ? Color.white : Color.gray;
    }
    private void Update()
    {
        if (!transformToHighlight.Enabled) return;

        float _percentage = Mathf.Repeat(Time.time * data.highlightFrequency, 1);
        float _addedXScale = data.highlightCurveX.Evaluate(_percentage) * data.highlightScaleMagnitude;
        float _addedYScale = data.highlightCurveY.Evaluate(_percentage) * data.highlightScaleMagnitude;
        transformToHighlight.Value.localScale = Vector3.one.WithAdded(_x: _addedXScale, _y: _addedYScale);

    }
}
