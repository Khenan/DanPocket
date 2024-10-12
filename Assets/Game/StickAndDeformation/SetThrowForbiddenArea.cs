using UnityEngine;

public class SetThrowForbiddenArea : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out Ball _ball))
        {
            _ball.SetCanThrow(false);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out Ball _ball))
        {
            _ball.SetCanThrow(true);
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.TryGetComponent(out Ball _ball))
        {
            _ball.SetCanThrow(false);
        }
    }
    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.TryGetComponent(out Ball _ball))
        {
            _ball.SetCanThrow(true);
        }
    }
}
