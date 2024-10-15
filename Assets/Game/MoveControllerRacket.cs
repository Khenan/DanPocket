using Umeshu.Uf;
using Umeshu.USystem.Time;
using UnityEngine;

public class MoveControllerRacket : MonoBehaviour
{
    private Rigidbody2D rb2D;
    [SerializeField] private float speedRotation = 1f;
    [SerializeField] private float currentRotation = 0f;
    [SerializeField] private Direction direction;
    private bool racketMove = false;
    private float DeltaTime => TimeManager.GetDeltaTime(TimeThread.Player);

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        LevelManager.Instance.AddMoveControllerRacket(this);
        LevelManager.Instance.OnRacketMovement += OnRacketMovement;
    }

    private void OnDisable()
    {
        LevelManager.Instance.RemoveMoveControllerRacket(this);
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
        if (racketMove)
        {
            float _rotation = speedRotation * DeltaTime;
            currentRotation += direction == Direction.Left ? _rotation : -_rotation;
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
