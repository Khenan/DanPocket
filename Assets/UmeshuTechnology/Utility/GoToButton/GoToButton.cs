using System;
using System.Collections;
using System.Collections.Generic;
using Umeshu.Common;
using Umeshu.Uf;
using Umeshu.USystem;
using Umeshu.Utility;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

[RequireComponent(typeof(GoToButton_ButtonLink))]
public abstract class GoToButton : MonoBehaviour
{

    public event Action onButtonAction;
    public abstract void PreClickDataSetup();
    public GoToButton_ButtonLink ButtonLink { get; private set; }

    private void Awake() => ButtonLink = GetComponent<GoToButton_ButtonLink>();


    public void ButtonGotClicked()
    {
        PreClickDataSetup();
        onButtonAction?.Invoke();
    }
}

public class GoToButtonLink<T> where T : GoToButton
{
    public T[] Buttons { get; private set; }
    public GameElement parent;
    public Action methodToCall;
    public void Init(GameElement _parent, Action _methodToCall, bool _includeInactive = false)
    {
        parent = _parent;
        if (parent != null) parent.onDestroy += Reset;
        methodToCall = _methodToCall;
        Buttons = Object.FindObjectsOfType<T>(_includeInactive);
        foreach (T _goToButton in Buttons)
            _goToButton.onButtonAction += methodToCall;
    }

    public void Reset()
    {
        if (parent != null) parent.onDestroy -= Reset;
        parent = null;
        foreach (T _goToButton in Buttons)
            _goToButton.onButtonAction -= methodToCall;
        Buttons = null;
    }

    public void ReGetButtons(GameElement _parent, bool _includeInactive = false)
    {
        Reset();
        Init(_parent, methodToCall, _includeInactive);
    }
}