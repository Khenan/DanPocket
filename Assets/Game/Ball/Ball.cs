using UnityEngine;

public class Ball : MonoBehaviour
{
    Rigidbody2D rb;
    StickAndDeformation stickAndDeformation;
    private const float maxMagnitude = 30f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stickAndDeformation = GetComponent<StickAndDeformation>();
    }

    private void Update()
    {
        if (rb.velocity.magnitude > maxMagnitude)
        {
            rb.velocity = rb.velocity.normalized * maxMagnitude;
        }
    }

    public void SetCanStick(bool _canStick)
    {
        stickAndDeformation.SetCanStick(_canStick);
    }
}
