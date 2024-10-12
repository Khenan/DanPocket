using System;
using UnityEngine;

public class Ball : MonoBehaviour
{
    Rigidbody2D rb;
    StickAndDeformation stickAndDeformation;
    ScoreBall scoreBall;
    private const float maxMagnitude = 30f;

    // ThrowBall
    [SerializeField] private Transform throwDirectionVisual;
    [SerializeField] private Transform throwCancelVisual;
    private Vector3 lastVelocity;
    private float lastMagnitude;
    private Vector3 startPosition;
    private Vector3 direction;
    private bool canThrow = true;
    public bool CanThrow => canThrow;
    private bool onThrowing = false;
    public bool OnThrowing => onThrowing;
    private int throwBallCount = 0;
    private int throwBallCountMax = 1;
    private float distanceRequiredToThrow = 1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stickAndDeformation = GetComponent<StickAndDeformation>();
        scoreBall = GetComponent<ScoreBall>();
        if (throwDirectionVisual != null) throwDirectionVisual.gameObject.SetActive(false);
        if (throwCancelVisual != null) throwCancelVisual.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (rb.velocity.magnitude > maxMagnitude)
        {
            rb.velocity = rb.velocity.normalized * maxMagnitude;
        }

        InputUpdate();
    }

    private void InputUpdate()
    {
        if (canThrow && throwBallCount < throwBallCountMax)
        {
            if (Input.GetMouseButtonDown(0) && onThrowing == false)
            {
                if (throwDirectionVisual != null) throwDirectionVisual.gameObject.SetActive(true);
                onThrowing = true;
                stickAndDeformation.SetCanStick(false);
                stickAndDeformation.CanSetKinematic = false;
                lastVelocity = rb.velocity;
                lastMagnitude = rb.velocity.magnitude;
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0;
                rb.isKinematic = true;
                startPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            else if (Input.GetMouseButton(0) && onThrowing)
            {
                Vector3 _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                direction = _mousePosition - startPosition;
                
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0;

                if (throwCancelVisual != null) throwCancelVisual.gameObject.SetActive(direction.magnitude < distanceRequiredToThrow);
                if (throwDirectionVisual != null)
                {
                    throwDirectionVisual.gameObject.SetActive(direction.magnitude >= distanceRequiredToThrow);
                    throwDirectionVisual.up = direction.normalized;
                }
            }
            else if (Input.GetMouseButtonUp(0) && onThrowing)
            {
                if (direction.magnitude > 1f)
                {
                    rb.velocity = direction.normalized * lastMagnitude;
                    throwBallCount++;
                }
                else rb.velocity = lastVelocity;

                stickAndDeformation.SetCanStick(true);
                rb.isKinematic = false;
                onThrowing = false;
                stickAndDeformation.CanSetKinematic = true;
                if (throwDirectionVisual != null) throwDirectionVisual.gameObject.SetActive(false);
                if (throwCancelVisual != null) throwCancelVisual.gameObject.SetActive(false);
            }
        }
        else
        {
            if (throwDirectionVisual != null) throwDirectionVisual.gameObject.SetActive(false);
            if (throwCancelVisual != null) throwCancelVisual.gameObject.SetActive(false);
        }
    }

    public void SetCanThrow(bool _canThrow)
    {
        canThrow = _canThrow;
        if (_canThrow) throwBallCount = 0;
    }
    public void SetCanStick(bool _canStick)
    {
        stickAndDeformation.SetCanStick(_canStick);
    }
    public uint GetScore()
    {
        return scoreBall.Score;
    }
}
