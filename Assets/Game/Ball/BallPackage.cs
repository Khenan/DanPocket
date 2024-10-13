using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPackage : MonoBehaviour, IGameElementManager
{
    [HideInInspector] public Ball ball;
    [HideInInspector] public ThrowControler throwControler;
    [HideInInspector] public StickAndDeformation stickAndDeformation;
    [HideInInspector] public ScoreBall scoreBall;

    private List<IGameElementComponent> gameComponents = new List<IGameElementComponent>();

    private void Awake()
    {
        ball = GetComponentInChildren<Ball>();
        stickAndDeformation = GetComponentInChildren<StickAndDeformation>();
        scoreBall = GetComponentInChildren<ScoreBall>();
        throwControler = GetComponentInChildren<ThrowControler>();

        IGameElementComponent[] _components = GetComponentsInChildren<IGameElementComponent>();
        gameComponents.AddRange(_components);
        gameComponents.ForEach(_component => _component.InitGameElementManager(this));
        gameComponents.ForEach(_component => _component.ComponentAwake());
    }

    private void Start()
    {
        gameComponents.ForEach(_component => _component.ComponentStart());
    }

    private void OnEnable()
    {
        gameComponents.ForEach(_component => _component.ComponentOnEnable());
    }

    private void OnDisable()
    {
        gameComponents.ForEach(_component => _component.ComponentOnDisable());
    }

    private void Update()
    {
        gameComponents.ForEach(_component => _component.ComponentUpdate());
    }

    private void FixedUpdate()
    {
        gameComponents.ForEach(_component => _component.ComponentFixedUpdate());
    }
}
