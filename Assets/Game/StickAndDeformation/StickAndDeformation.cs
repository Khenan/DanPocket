using System.Linq;
using Umeshu.Uf;
using Umeshu.USystem;
using Umeshu.USystem.Time;
using UnityEngine;

internal class StickAndDeformation : MonoBehaviour
{
    [Header("Rigidbody")]
    [SerializeField] private Rigidbody2D elementRigidbody2D;

    [Header("Roots")]
    [SerializeField] private Transform parentRoot;
    [SerializeField] private Transform deformationRoot;
    [SerializeField] private Transform rerotateRoot;

    [Header("Deformation")]
    [SerializeField] private float speedMaxRef = 200f;
    [SerializeField, Range(0f, .9f)] private float scaleDeformationAtMaxSpeedPercentage = .4f;
    [SerializeField, Range(0f, .9f)] private float scaleDeformationAtMaxStickPercentage = .8f;
    [SerializeField] private float deformationLerpSpeed = 15;

    [Header("Stick on impact")]
    [SerializeField, Range(0.001f, 1f)] private float stickReductionFromDot_min = 0.1f;
    [SerializeField, Range(0.001f, 1f)] private float stickReductionFromDot_max = 0.4f;
    [SerializeField, Range(1, 50)] private float minSpeedDifferenceToStick = 5f;
    [SerializeField, Range(1, 50)] private float maxSpeedDifferenceToStick = 20f;
    [SerializeField, Range(0.001f, 2f)] private float minStickDuration = .25f;
    [SerializeField, Range(0.001f, 2f)] private float maxStickDuration = .5f;
    [SerializeField] private AnimationCurve impactAnim = AnimationCurve.Linear(0, 0, 1, 1);

    // --- Deformation
    private float currentDeformation = 0f;
    private float lastDeformation = 0f;
    private Transform deformationRootParent = null;

    // --- Stick
    internal bool IsSticked => stickPercentage < 1;

    public float DeltaTime => TimeManager.GetDeltaTime(TimeThread.Player);

    public bool CanSetKinematic = true;
    private Vector2 velocityStockedForStick = Vector2.zero;
    private float angularVelocityStockedForStick = 0;
    private Vector2 stickFacingVector = Vector2.up;
    [SerializeField] private float stickPercentage = 0f;
    private float stickDuration = 0f;
    private float stickDeformationPercentage = 0f;
    private bool stickIsFromProjection = false;
    private const float STICK_DOT_FROM_PROJECTION = 0.5f;

    // --- Stick Detection
    private Vector2 velocityLastFrame = Vector2.zero;

    // --- Stick Control
    private bool canStick = true;

#if UNITY_EDITOR
    // --- Debug
    private Vector3 debug_stickPastVector = Vector2.zero;
    private Vector3 debug_stickFacingVector = Vector2.zero;
    private Vector3 debug_stickNextVector = Vector2.zero;
    private Vector3 debug_lastStickPosition = Vector2.zero;
#endif


    private void Awake()
    {
        deformationRootParent = deformationRoot.parent;
    }

    private void OnEnable()
    {
        deformationRoot.localScale = Vector3.one;
        deformationRoot.localPosition = Vector3.zero;
        stickPercentage = 1;
        currentDeformation = 0;
        velocityLastFrame = Vector2.zero;
    }

    private void Update()
    {
        if (elementRigidbody2D != null && IsSticked && CanSetKinematic) elementRigidbody2D.isKinematic = true;
        else if (elementRigidbody2D != null && CanSetKinematic) elementRigidbody2D.isKinematic = false;
        ScaleAndRotate();
        UpdateStick();

#if UNITY_EDITOR
        Debug.DrawLine(debug_lastStickPosition, debug_lastStickPosition + debug_stickPastVector * 3, Color.red);
        Debug.DrawLine(debug_lastStickPosition, debug_lastStickPosition + debug_stickFacingVector * 3, Color.blue);
        Debug.DrawLine(debug_lastStickPosition, debug_lastStickPosition + debug_stickNextVector * 3, Color.green);
#endif
    }

    private void FixedUpdate()
    {
        DetectStick();
    }

    private void DetectStick()
    {
        if (IsSticked) return;

        Vector2 _velocityLastFrame = velocityLastFrame;
        Vector2 _velocityThisFrame = elementRigidbody2D.velocity;
        Vector2 _velocityDiference = _velocityThisFrame - _velocityLastFrame;
        velocityLastFrame = _velocityThisFrame;

        Stick(_velocityLastFrame, _velocityThisFrame, _velocityDiference, elementRigidbody2D.angularVelocity);
    }

    private void Stick(Vector2 _velocityLastFrame, Vector2 _velocityThisFrame, Vector2 _velocityDiference, float _angularVelocity)
    {
        float _usedMagnitudeForCalculations = _velocityDiference.magnitude;

        float _percentageOfStickToApply_FromVelocity = _usedMagnitudeForCalculations.Remap(minSpeedDifferenceToStick, maxSpeedDifferenceToStick, _newA: 0, _newB: 1);

        float _usedDot = Vector2.Dot(_velocityLastFrame.normalized, _velocityThisFrame.normalized);

        float _percentageOfStickToApply_FromDot = stickIsFromProjection ? STICK_DOT_FROM_PROJECTION : (-_usedDot).Remap(stickReductionFromDot_min, stickReductionFromDot_max, _newA: 0, _newB: 1);

        stickIsFromProjection = false;

        float _percentageOfStickToApply = _percentageOfStickToApply_FromVelocity * _percentageOfStickToApply_FromDot;

        bool _shouldStick = _percentageOfStickToApply > 0;
        if (_shouldStick && canStick)
        {
            Vector2 _correctedVelocityLastFrame = -_velocityLastFrame;
            float _magnitudeLastFrame = _correctedVelocityLastFrame.magnitude;
            float _magnitudeThisFrame = _velocityThisFrame.magnitude;

            float _percentageOfFacingPastDirection = Mathf.Clamp01(_magnitudeLastFrame / (_magnitudeLastFrame + _magnitudeThisFrame));
            stickFacingVector = Vector2.Lerp(_correctedVelocityLastFrame, _velocityThisFrame, _percentageOfFacingPastDirection).normalized;

#if UNITY_EDITOR
            debug_lastStickPosition = parentRoot.position;
            debug_stickPastVector = _correctedVelocityLastFrame;
            debug_stickFacingVector = stickFacingVector;
            debug_stickNextVector = _velocityThisFrame;
#endif

            velocityStockedForStick = _velocityThisFrame;
            angularVelocityStockedForStick = _angularVelocity;
            stickPercentage = 0;
            stickDuration = Mathf.Lerp(minStickDuration, maxStickDuration, _percentageOfStickToApply);
            stickDeformationPercentage = scaleDeformationAtMaxStickPercentage * -_percentageOfStickToApply;
            elementRigidbody2D.velocity = Vector2.zero;
            elementRigidbody2D.angularVelocity = 0;
        }
    }

    public void SetCanStick(bool _canStick)
    {
        canStick = _canStick;
    }

    private void UpdateStick()
    {
        if (!IsSticked) return;
        UfMath.MoveTowards(ref stickPercentage, _aimedValue: 1f, DeltaTime / stickDuration, ApplyStockedVelocity);
    }

    private void ApplyStockedVelocity()
    {
        elementRigidbody2D.velocity = velocityStockedForStick;
        elementRigidbody2D.angularVelocity = angularVelocityStockedForStick;
    }


    private void ScaleAndRotate()
    {
        if (deformationRoot != null && rerotateRoot != null)
        {
            Vector2 _usedDir = IsSticked ? stickFacingVector * velocityStockedForStick.magnitude : elementRigidbody2D.velocity;

            deformationRoot.right = _usedDir.normalized;
            rerotateRoot.up = transform.up;

            float _percentageToMaxSpeed = Mathf.Clamp01(_usedDir.magnitude / speedMaxRef);
            float _currScaleAdded = _percentageToMaxSpeed * scaleDeformationAtMaxSpeedPercentage;
            currentDeformation = Mathf.Lerp(currentDeformation, _currScaleAdded, DeltaTime * deformationLerpSpeed);

            if (IsSticked)
            {
                float _impactAnimEval = impactAnim.Evaluate(stickPercentage);
                currentDeformation = Mathf.Lerp(currentDeformation, stickDeformationPercentage, _impactAnimEval);
                float _decal = Mathf.Lerp(0, stickDeformationPercentage, _impactAnimEval);
                deformationRoot.position = deformationRootParent.transform.position + deformationRoot.right * _decal;
            }
            else
            {
                deformationRoot.localPosition = Vector3.Lerp(deformationRoot.localPosition, Vector3.zero, DeltaTime * deformationLerpSpeed);
            }

            float _usedDeformation = currentDeformation;
            deformationRoot.localScale = new Vector3(1 + _usedDeformation, 1 - _usedDeformation, 1);
            lastDeformation = _usedDeformation;
        }
    }
}
