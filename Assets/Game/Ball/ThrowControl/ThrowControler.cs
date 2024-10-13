using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowControler : MonoBehaviour, IGameElementComponent
{
    private BallPackage ballPackage;
    private Rigidbody2D rb => ballPackage.ball.rb;
    private StickAndDeformation stickAndDeformation => ballPackage.stickAndDeformation;

    [Header("Joysticks")]
    [SerializeField] private Transform throwJoystickVisualRoot;
    [SerializeField] private Transform throwJoystickVisualPosition;

    [Header("Others")]
    [SerializeField] private Transform throwDirectionVisual;
    [SerializeField] private Transform throwCancelVisual;
    [SerializeField] private Transform aura;

    private Vector3 lastVelocity;
    private float lastMagnitude;
    private float minMagnitude = Ball.maxMagnitude / 2;
    private Vector3 startPosition;
    private Vector3 direction;
    private bool canThrow = true;
    public bool CanThrow => canThrow;
    private bool onThrowing = false;
    public bool OnThrowing => onThrowing;
    private int throwBallCount = 0;
    private int throwBallCountMax = 1;
    private float distanceRequiredToThrow = 1f;

    public void InitGameElementManager(IGameElementManager _gameElementManager)
    {
        ballPackage = (BallPackage)_gameElementManager;
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

                if (throwDirectionVisual != null) throwDirectionVisual.position = ballPackage.ball.transform.position;
                if (throwCancelVisual != null) throwCancelVisual.position = ballPackage.ball.transform.position;
                if (throwJoystickVisualRoot != null)
                {
                    throwJoystickVisualRoot.position = startPosition.With(_z: 0);
                    throwJoystickVisualRoot.gameObject.SetActive(true);
                }
            }
            else if (Input.GetMouseButton(0) && onThrowing)
            {
                Vector3 _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                direction = _mousePosition - startPosition;

                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0;

                if (throwCancelVisual != null)
                {
                    throwCancelVisual.position = ballPackage.ball.transform.position;
                    throwCancelVisual.gameObject.SetActive(direction.magnitude < distanceRequiredToThrow);
                }
                if (throwDirectionVisual != null)
                {
                    throwDirectionVisual.position = ballPackage.ball.transform.position;
                    throwDirectionVisual.gameObject.SetActive(direction.magnitude >= distanceRequiredToThrow);
                    throwDirectionVisual.up = direction.normalized;
                }
                if (throwJoystickVisualPosition != null) throwJoystickVisualPosition.localPosition = direction.normalized * Mathf.Min(direction.magnitude, 2f);
                if (throwJoystickVisualRoot != null) throwJoystickVisualRoot.gameObject.SetActive(true);
            }
            else if (Input.GetMouseButtonUp(0) && onThrowing)
            {
                if (direction.magnitude > 1f)
                {
                    if (lastMagnitude < minMagnitude) lastMagnitude = minMagnitude;
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
                if (throwJoystickVisualRoot != null) throwJoystickVisualRoot.gameObject.SetActive(false);
            }
        }
        else
        {
            if (throwDirectionVisual != null) throwDirectionVisual.gameObject.SetActive(false);
            if (throwCancelVisual != null) throwCancelVisual.gameObject.SetActive(false);
            if (throwJoystickVisualRoot != null) throwJoystickVisualRoot.gameObject.SetActive(false);
        }
    }
    public void SetCanThrow(bool _canThrow)
    {
        canThrow = _canThrow;
        if (_canThrow) throwBallCount = 0;
    }

    public void ComponentAwake()
    {
        if (throwDirectionVisual != null) throwDirectionVisual.gameObject.SetActive(false);
        if (throwCancelVisual != null) throwCancelVisual.gameObject.SetActive(false);
    }

    public void ComponentStart() { }

    public void ComponentOnEnable() { }

    public void ComponentOnDisable() { }

    public void ComponentUpdate()
    {
        InputUpdate();
        if (aura != null)
        {
            aura.position = ballPackage.ball.transform.position;
            aura.gameObject.SetActive(canThrow && throwBallCount < throwBallCountMax);
        }
    }

    public void ComponentFixedUpdate() { }
}
