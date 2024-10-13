using log4net.Core;
using Umeshu.Uf;
using Umeshu.USystem;
using UnityEngine;
using UnityEngine.UI;

public class MoveControllerRacket : MonoBehaviour
{
    private Rigidbody2D rb2D;
    [SerializeField] private float speedRotation = 1f;
    [SerializeField] private float currentRotation = 0f;
    [SerializeField] private Direction direction;
    private bool racketMove = false;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        LevelManager.Instance.AddMoveControllerRacket(direction, this);
        LevelManager.Instance.OnRacketMovement += OnRacketMovement;
    }

    private void OnDisable()
    {
        LevelManager.Instance.RemoveMoveControllerRacket(direction, this);
        LevelManager.Instance.OnRacketMovement -= OnRacketMovement;
    }

    private void OnRacketMovement(Direction _direction)
    {
        if(_direction == direction) {
            racketMove = true;
        }
    }
    private void Update()
    {
        if (racketMove && direction == Direction.Left)
        {
            currentRotation += speedRotation * Time.deltaTime;
        }
        else if (racketMove && direction == Direction.Right)
        {
            currentRotation -= speedRotation * Time.deltaTime;
        }
        else
        {
            currentRotation = direction == Direction.Left ? -30 : 30;
        }
        currentRotation = Mathf.Clamp(currentRotation, -30, 30);
        rb2D.MoveRotation(currentRotation);
        racketMove = false;
    }
}
