using System;
using System.Collections.Generic;
using Umeshu.Uf;
using Umeshu.USystem.Time;
using UnityEngine;

public class LevelManager : MonoBehaviour, ITimeSpeedMultiplierModifier
{
    public static LevelManager Instance;

    public Action<Direction> OnRacketMovement;
    private List<MoveControllerRacket> moveControllerRackets = new();

    private float wantedSpeedMultiplier = 1f;

    public float GetWantedSpeedMultiplier() => wantedSpeedMultiplier;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void OnEnable()
    {
        TimeManager.SuscribeModifier(TimeThread.Player, this);
    }
    private void OnDisable()
    {
        TimeManager.UnsuscribeModifier(TimeThread.Player, this);
    }

    public void AddMoveControllerRacket(MoveControllerRacket _moveControllerRacket)
    {
        moveControllerRackets.Add(_moveControllerRacket);
    }

    public void RemoveMoveControllerRacket(MoveControllerRacket _moveControllerRacket)
    {
        moveControllerRackets.Remove(_moveControllerRacket);
    }

    public void MoveRackets(Direction _direction)
    {
        OnRacketMovement?.Invoke(_direction);
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 _mousePosition = Input.mousePosition;
            int _width = Camera.main.pixelWidth;
            if (_mousePosition.x <= _width / 2) MoveRackets(Direction.Left);
            else MoveRackets(Direction.Right);
        }
    }
}
