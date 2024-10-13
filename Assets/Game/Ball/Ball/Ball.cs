using System;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour, IGameElementComponent
{
    [HideInInspector] public BallPackage ballPackage;
    [HideInInspector] public Rigidbody2D rb;
    public const float maxMagnitude = 30f;
    public const float maxAngularSpeed = 360f;

    public void InitGameElementManager(IGameElementManager _gameElementManager)
    {
        ballPackage = (BallPackage)_gameElementManager;
    }

    public void ComponentAwake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void ComponentStart() { }

    public void ComponentOnEnable() { }

    public void ComponentOnDisable() { }

    public void ComponentUpdate() { }

    public void ComponentFixedUpdate()
    {
        if (rb.velocity.magnitude > maxMagnitude)
        {
            rb.velocity = rb.velocity.normalized * maxMagnitude;
        }
        if (Mathf.Abs(rb.angularVelocity) > maxAngularSpeed)
        {
            rb.angularVelocity = Mathf.Sign(rb.angularVelocity) * maxAngularSpeed;
        }
    }

    public void SetCanThrow(bool _canThrow)
    {
        ballPackage.throwControler.SetCanThrow(_canThrow);
    }
    public void SetCanStick(bool _canStick)
    {
        ballPackage.stickAndDeformation.SetCanStick(_canStick);
    }
    public uint GetScore()
    {
        return ballPackage.scoreBall.Score;
    }
}
