using UnityEngine;

public class MoveControllerRaket : MonoBehaviour
{
    private Rigidbody2D rb2D;
    [SerializeField] private float speedRotation = 1f;
    [SerializeField] private float currentRotation = 0f;
    private enum Direction { Left, Right }
    [SerializeField] private Direction direction;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && direction == Direction.Left)
        {
            currentRotation += speedRotation * Time.deltaTime;
        }
        else if (Input.GetMouseButton(1) && direction == Direction.Right)
        {
            currentRotation -= speedRotation * Time.deltaTime;
        }
        else
        {
            currentRotation = direction == Direction.Left ? -30 : 30;
        }
        currentRotation = Mathf.Clamp(currentRotation, -30, 30);
        rb2D.MoveRotation(currentRotation);
    }
}
