using UnityEngine;

public class SetStickArea : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out StickAndDeformation _stickAndDeformation))
        {
            _stickAndDeformation.SetCanStick(false);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out StickAndDeformation _stickAndDeformation))
        {
            _stickAndDeformation.SetCanStick(true);
        }
    }
    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.TryGetComponent(out StickAndDeformation _stickAndDeformation))
        {
            _stickAndDeformation.SetCanStick(false);
        }
    }
    private void OnCollisionExit2D(Collision2D other) {
        if (other.gameObject.TryGetComponent(out StickAndDeformation _stickAndDeformation))
        {
            _stickAndDeformation.SetCanStick(true);
        }
    }
}
