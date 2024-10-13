using System;
using System.Collections.Generic;
using Umeshu.Uf;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public Action<Direction> OnRacketMovement;
    private Dictionary<Direction, List<MoveControllerRacket>> moveControllerRackets = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddMoveControllerRacket(Direction _direction, MoveControllerRacket _moveControllerRacket)
    {
        if (!moveControllerRackets.ContainsKey(_direction))
        {
            moveControllerRackets.Add(_direction, new List<MoveControllerRacket>());
        }
        else
        {
            moveControllerRackets[_direction].Add(_moveControllerRacket);
        }
    }

    public void RemoveMoveControllerRacket(Direction _direction, MoveControllerRacket _moveControllerRacket)
    {
        if (moveControllerRackets.ContainsKey(_direction))
        {
            moveControllerRackets[_direction].Remove(_moveControllerRacket);
        }
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
            Camera.main.pixelWidth.LogDesc("Camera width");
            _mousePosition.x.LogDesc("Mouse position x");
            if (_mousePosition.x <= _width / 2)
            {
                "Move left".Log();
                MoveRackets(Direction.Left);
            }
            else
            {
                "Move right".Log();
                MoveRackets(Direction.Right);
            }
        }
    }
}
