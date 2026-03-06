using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BirdGlideController : MonoBehaviour
{
    [Header("Flap")]
    public float flapUpImpulse = 6.5f;
    public float flapForwardImpulse = 2.0f;
    public float flapCooldown = 0.10f;

    [Header("Forward Motion")]
    public float forwardThrust = 12f;
    public float diveForwardBonus = 10f;
    public float maxForwardSpeed = 18f;

    [Header("Gravity")]
    public float extraFallGravity = 3.0f;
    public float extraRiseGravity = 1.0f;
    public float maxUpSpeed = 7f;
    public float maxDownSpeed = 12f;

    [Header("Glide")]
    public float baseLinearDamping = 0.20f;
    public float glideLinearDamping = 0.55f;
    public float lift = 4.5f;
    public float minLift = 0.0f;
    public float maxLift = 8f;

    [Header("Turning / Redirect")]
    public float minimumForwardBias = 2f;

    [Tooltip("Redirection strength at very low speed.")]
    public float minTurnRedirectStrength = 4f;

    [Tooltip("Redirection strength at high speed.")]
    public float maxTurnRedirectStrength = 14f;

    [Tooltip("Sideways slip damping at very low speed.")]
    public float minSidewaysDamping = 6f;

    [Tooltip("Sideways slip damping at high speed.")]
    public float maxSidewaysDamping = 22f;

    [Header("Rotation")]
    public float pitchSpeed = 95f;
    public float rollSpeed = 140f;

    [Tooltip("Yaw turning rate at low speed.")]
    public float minYawSpeedFromRoll = 20f;

    [Tooltip("Yaw turning rate at high speed.")]
    public float maxYawSpeedFromRoll = 95f;

    public float pitchLimit = 40f;
    public float rollLimit = 50f;
    public float rotationSmoothing = 12f;

    [Header("Auto-Level")]
    public float pitchReturnSpeed = 40f;
    public float rollReturnSpeed = 80f;

    private Rigidbody rb;
    private float lastFlapTime = -999f;

    private float targetPitch;
    private float targetRoll;
    private float targetYaw;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.linearDamping = baseLinearDamping;
        rb.angularDamping = 2f;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        Vector3 e = transform.eulerAngles;
        targetPitch = NormalizeAngle(e.x);
        targetYaw = NormalizeAngle(e.y);
        targetRoll = NormalizeAngle(e.z);
    }

    void Update()
    {
        HandleInput();
    }

    void FixedUpdate()
    {
        ApplyRotation();
        ApplyForwardMotion();
        ApplyTurnRedirection();
        ApplyBetterGravity();
        ApplyGlideForces();
        ClampVelocity();
    }

    void HandleInput()
    {
        bool pitching = false;
        bool rolling = false;

        // W = pitch forward (nose down)
        if (Input.GetKey(KeyCode.W))
        {
            targetPitch += pitchSpeed * Time.deltaTime;
            pitching = true;
        }

        // S = pitch backward (nose up)
        if (Input.GetKey(KeyCode.S))
        {
            targetPitch -= pitchSpeed * Time.deltaTime;
            pitching = true;
        }

        // A = tilt left
        if (Input.GetKey(KeyCode.A))
        {
            targetRoll += rollSpeed * Time.deltaTime;
            rolling = true;
        }

        // D = tilt right
        if (Input.GetKey(KeyCode.D))
        {
            targetRoll -= rollSpeed * Time.deltaTime;
            rolling = true;
        }

        if (!pitching)
            targetPitch = Mathf.MoveTowards(targetPitch, 0f, pitchReturnSpeed * Time.deltaTime);

        if (!rolling)
            targetRoll = Mathf.MoveTowards(targetRoll, 0f, rollReturnSpeed * Time.deltaTime);

        targetPitch = Mathf.Clamp(targetPitch, -pitchLimit, pitchLimit);
        targetRoll = Mathf.Clamp(targetRoll, -rollLimit, rollLimit);

        // Speed-based turning:
        // faster forward speed = stronger yaw response from banking
        float speed01 = GetForwardSpeed01();
        float yawSpeedFromRoll = Mathf.Lerp(minYawSpeedFromRoll, maxYawSpeedFromRoll, speed01);

        float rollFraction = targetRoll / rollLimit;
        targetYaw -= rollFraction * yawSpeedFromRoll * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space))
            TryFlap();
    }

    void TryFlap()
    {
        if (Time.time - lastFlapTime < flapCooldown) return;
        lastFlapTime = Time.time;

        Vector3 v = rb.linearVelocity;

        if (v.y < 0f)
            v.y *= 0.5f;

        rb.linearVelocity = v;

        Vector3 flapForce = (Vector3.up * flapUpImpulse) + (transform.forward * flapForwardImpulse);
        rb.AddForce(flapForce, ForceMode.Impulse);
    }

    void ApplyRotation()
    {
        Quaternion targetRotation = Quaternion.Euler(targetPitch, targetYaw, targetRoll);
        Quaternion smoothedRotation = Quaternion.Slerp(
            rb.rotation,
            targetRotation,
            1f - Mathf.Exp(-rotationSmoothing * Time.fixedDeltaTime)
        );

        rb.MoveRotation(smoothedRotation);
    }

    void ApplyForwardMotion()
    {
        Vector3 vel = rb.linearVelocity;
        float currentForwardSpeed = Vector3.Dot(vel, transform.forward);

        // 0 when nose up, 1 when nose down
        float diveFactor = Mathf.InverseLerp(-pitchLimit, pitchLimit, targetPitch);
        float thrust = forwardThrust + (diveForwardBonus * diveFactor);

        if (currentForwardSpeed < maxForwardSpeed)
            rb.AddForce(transform.forward * thrust, ForceMode.Acceleration);
    }

    void ApplyTurnRedirection()
    {
        Vector3 vel = rb.linearVelocity;

        float verticalSpeed = vel.y;
        Vector3 horizontalVel = new Vector3(vel.x, 0f, vel.z);

        if (horizontalVel.sqrMagnitude < 0.0001f)
            horizontalVel = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized * minimumForwardBias;

        Vector3 flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        if (flatForward.sqrMagnitude < 0.0001f)
            flatForward = transform.forward.normalized;

        float horizontalSpeed = horizontalVel.magnitude;
        float speed01 = Mathf.Clamp01(horizontalSpeed / maxForwardSpeed);

        // Faster speed = stronger redirection and less sideways slip
        float turnRedirectStrength = Mathf.Lerp(minTurnRedirectStrength, maxTurnRedirectStrength, speed01);
        float sidewaysDamping = Mathf.Lerp(minSidewaysDamping, maxSidewaysDamping, speed01);

        Vector3 desiredHorizontalVel = flatForward * Mathf.Max(horizontalSpeed, minimumForwardBias);

        horizontalVel = Vector3.Lerp(
            horizontalVel,
            desiredHorizontalVel,
            1f - Mathf.Exp(-turnRedirectStrength * Time.fixedDeltaTime)
        );

        float sidewaysSpeed = Vector3.Dot(horizontalVel, transform.right);
        horizontalVel -= transform.right * sidewaysSpeed * (1f - Mathf.Exp(-sidewaysDamping * Time.fixedDeltaTime));

        rb.linearVelocity = new Vector3(horizontalVel.x, verticalSpeed, horizontalVel.z);
    }

    void ApplyBetterGravity()
    {
        float extraGravity = rb.linearVelocity.y < 0f ? extraFallGravity : extraRiseGravity;
        rb.AddForce(Vector3.down * extraGravity, ForceMode.Acceleration);
    }

    void ApplyGlideForces()
    {
        Vector3 vel = rb.linearVelocity;
        float forwardSpeed = Vector3.Dot(vel, transform.forward);
        forwardSpeed = Mathf.Max(0f, forwardSpeed);

        // 0 when nose down, 1 when nose up
        float glideFactor = Mathf.InverseLerp(pitchLimit, -pitchLimit, targetPitch);

        float liftAccel = minLift + (forwardSpeed * lift * glideFactor * 0.12f);
        liftAccel = Mathf.Clamp(liftAccel, 0f, maxLift);

        rb.AddForce(Vector3.up * liftAccel, ForceMode.Acceleration);
        rb.linearDamping = Mathf.Lerp(baseLinearDamping, glideLinearDamping, glideFactor);
    }

    void ClampVelocity()
    {
        Vector3 v = rb.linearVelocity;

        v.y = Mathf.Clamp(v.y, -maxDownSpeed, maxUpSpeed);

        float forwardComponent = Vector3.Dot(v, transform.forward);
        if (forwardComponent > maxForwardSpeed)
        {
            float excess = forwardComponent - maxForwardSpeed;
            v -= transform.forward * excess;
        }

        rb.linearVelocity = v;
    }

    float GetForwardSpeed01()
    {
        float forwardSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);
        forwardSpeed = Mathf.Max(0f, forwardSpeed);
        return Mathf.Clamp01(forwardSpeed / maxForwardSpeed);
    }

    float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}