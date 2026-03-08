using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BirdGlideController : MonoBehaviour
{
    [Header("Debug / Telemetry")]
    public Vector3 CurrentVelocity; // updated every Update()

    [Header("Initial Launch")]
    [Tooltip("Local-space initial velocity: X=right, Y=up, Z=forward.")]
    public Vector3 initialVelocity = new Vector3(0f, 4f, 10f);

    [Header("Flap")]
    public float flapUpImpulse = 6.5f;
    public float flapForwardImpulse = 2.0f;

    [Tooltip("Seconds to wait after pressing Space before applying the flap impulse.")]
    public float flapDelay = 0.10f;

    public float flapCooldown = 0.10f;

    [Header("Forward Motion")]
    public float forwardThrust = 12f;
    public float diveForwardBonus = 10f;

    [Tooltip("Small forward bonus when nose is up.")]
    public float noseUpThrustBonus = 2.5f;

    public float maxForwardSpeed = 18f;

    [Header("Gravity")]
    public float extraFallGravity = 3.0f;
    public float extraRiseGravity = 1.0f;
    public float maxUpSpeed = 7f;
    public float maxDownSpeed = 12f;

    [Header("Damping")]
    public float baseLinearDamping = 0.20f;
    public float glideLinearDamping = 0.55f;

    [Header("Glider Pull-Up (Speed * Nose-Up)")]
    public float noseUpLift = 60f;
    [Range(0.5f, 4f)] public float speedLiftPower = 1.6f;
    [Range(0.7f, 3f)] public float noseUpPower = 1.0f;
    [Range(0f, 10f)] public float noseUpDeadzoneDegrees = 2f;
    public float maxLift = 40f;

    [Header("Energy Exchange")]
    public float pullUpForwardDrag = 14f;

    [Tooltip("How strongly pulling up rotates velocity toward bird forward (converts momentum to climb).")]
    public float pitchUpRedirectStrength = 12f;

    [Tooltip("Clamp redirect vertical spike.")]
    public float maxRedirectUpSpeed = 10f;

    [Header("Anti-Infinite-Climb (Smooth Stall)")]
    public float stallStartSpeed = 2.5f;
    public float stallSpeed = 7.5f;
    public float stallSinkStartSpeed = 1.2f;
    public float stallSinkForce = 6f;

    [Header("Turn Lift Fix")]
    [Range(0f, 1f)]
    public float bankLiftReduction = 0.65f;
    public float turnSinkForce = 1.5f;

    [Header("Turning / Redirect")]
    public float minimumForwardBias = 2f;
    public float minTurnRedirectStrength = 4f;
    public float maxTurnRedirectStrength = 14f;
    public float minSidewaysDamping = 6f;
    public float maxSidewaysDamping = 22f;

    [Header("Rotation")]
    public float pitchSpeed = 95f;
    public float rollSpeed = 140f;
    public float minYawSpeedFromRoll = 20f;
    public float maxYawSpeedFromRoll = 95f;
    public float pitchLimit = 40f;
    public float rollLimit = 50f;
    public float rotationSmoothing = 12f;

    [Header("Auto-Level")]
    public float pitchReturnSpeed = 40f;
    public float rollReturnSpeed = 80f;

    private Rigidbody rb;

    private float lastFlapFiredTime = -999f;

    private bool flapQueued = false;
    private float flapFireTime = 0f;

    private float targetPitch;
    private float targetRoll;
    private float targetYaw;

    private Animator animator;

    private bool hasStarted = false;
    private bool isDead = false;

    private AnimatorStateInfo stateInfo;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();

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

    private void Start()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public void BeginFlight()
    {
        hasStarted = true;
        isDead = false;

        transform.position = new Vector3(0f, 10f, 0f);
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 v =
            transform.right * initialVelocity.x +
            Vector3.up * initialVelocity.y +
            transform.forward * initialVelocity.z;

        rb.linearVelocity = v;
    }

    void Update()
    {
        CurrentVelocity = (rb != null) ? rb.linearVelocity : Vector3.zero;

        HandleInput();
        ProcessQueuedFlap();
    }

    void FixedUpdate()
    {
        ApplyRotation();
        ApplyForwardMotion();
        ApplyTurnRedirection();
        ApplyPitchUpVelocityRedirect();
        ApplyBetterGravity();
        ApplyGlideForces();
        ClampVelocity();
    }

    // ---- input / flap ----

    void HandleInput()
    {
        bool pitching = false;
        bool rolling = false;

        if (Input.GetKey(KeyCode.W)) 
        { 
            targetPitch += pitchSpeed * Time.deltaTime; pitching = true; 
        }
        if (Input.GetKey(KeyCode.S)) 
        { 
            targetPitch -= pitchSpeed * Time.deltaTime; pitching = true; 
        
            if (animator != null)
            {

                stateInfo = animator.GetCurrentAnimatorStateInfo(0);

                if (stateInfo.IsName("Do Nothing"))
                {
                    animator.speed=2f;
                    animator.Play("Flap", 0, 0f);
                }
            }
        }

        if (Input.GetKey(KeyCode.A)) 
        { 
            targetRoll += rollSpeed * Time.deltaTime; rolling = true; 

            if (animator != null)
            {

                stateInfo = animator.GetCurrentAnimatorStateInfo(0);

                if (stateInfo.IsName("Do Nothing"))
                {
                    animator.speed=2f;
                    animator.Play("Flap", 0, 0f);
                }
            }
        }

        if (Input.GetKey(KeyCode.D)) 
        { 
            targetRoll -= rollSpeed * Time.deltaTime; rolling = true; 

            if (animator != null)
            {

                stateInfo = animator.GetCurrentAnimatorStateInfo(0);

                if (stateInfo.IsName("Do Nothing"))
                {
                    animator.speed=2f;
                    animator.Play("Flap", 0, 0f);
                }
            }
        }

        if (!pitching) targetPitch = Mathf.MoveTowards(targetPitch, 0f, pitchReturnSpeed * Time.deltaTime);
        if (!rolling) targetRoll = Mathf.MoveTowards(targetRoll, 0f, rollReturnSpeed * Time.deltaTime);

        targetPitch = Mathf.Clamp(targetPitch, -pitchLimit, pitchLimit);
        targetRoll = Mathf.Clamp(targetRoll, -rollLimit, rollLimit);

        float speed01 = GetForwardSpeed01();
        float yawSpeedFromRoll = Mathf.Lerp(minYawSpeedFromRoll, maxYawSpeedFromRoll, speed01);

        float rollFraction = targetRoll / rollLimit;
        targetYaw -= rollFraction * yawSpeedFromRoll * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space))
            QueueFlap();
    }

    void QueueFlap()
    {
        float earliestAllowed = lastFlapFiredTime + flapCooldown;
        float desiredFireTime = Time.time + Mathf.Max(0f, flapDelay);
        float fireTime = Mathf.Max(desiredFireTime, earliestAllowed);

        if (!flapQueued)
        {
            flapQueued = true;
            flapFireTime = fireTime;

            if (animator != null)
            {

                stateInfo = animator.GetCurrentAnimatorStateInfo(0);

                if (stateInfo.IsName("Do Nothing"))
                {
                    animator.speed=1f;
                    animator.Play("Flap", 0, 0f);
                }
            }
        }
        else
        {
            flapFireTime = Mathf.Min(flapFireTime, fireTime);
        }
    }

    void ProcessQueuedFlap()
    {
        if (!flapQueued) return;
        if (Time.time < flapFireTime) return;

        flapQueued = false;
        flapFireTime = 0f;

        FireFlap();
    }

    void FireFlap()
    {
        lastFlapFiredTime = Time.time;

        Vector3 v = rb.linearVelocity;
        if (v.y < 0f) v.y *= 0.5f;
        rb.linearVelocity = v;

        Vector3 flapForce = (Vector3.up * flapUpImpulse) + (transform.forward * flapForwardImpulse);
        rb.AddForce(flapForce, ForceMode.Impulse);
    }

    // ---- physics ----

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
        Vector3 flatForward = GetFlatForward();
        Vector3 horizontalVel = new Vector3(vel.x, 0f, vel.z);

        float currentForwardSpeed = Vector3.Dot(horizontalVel, flatForward);

        float diveFactor = Mathf.InverseLerp(-pitchLimit, pitchLimit, targetPitch);
        float noseUp01 = GetNoseUp01();

        float thrust = forwardThrust
                     + (diveForwardBonus * diveFactor)
                     + (noseUpThrustBonus * noseUp01);

        if (currentForwardSpeed < maxForwardSpeed)
            rb.AddForce(flatForward * thrust, ForceMode.Acceleration);
    }

    void ApplyTurnRedirection()
    {
        Vector3 vel = rb.linearVelocity;

        float verticalSpeed = vel.y;
        Vector3 horizontalVel = new Vector3(vel.x, 0f, vel.z);

        Vector3 flatForward = GetFlatForward();
        if (horizontalVel.sqrMagnitude < 0.0001f)
            horizontalVel = flatForward * minimumForwardBias;

        float horizontalSpeed = horizontalVel.magnitude;
        float speed01 = Mathf.Clamp01(horizontalSpeed / maxForwardSpeed);

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

    void ApplyPitchUpVelocityRedirect()
    {
        float noseUp01 = GetNoseUp01();
        if (noseUp01 <= 0f) return;

        Vector3 v = rb.linearVelocity;
        float speed = v.magnitude;
        if (speed < 0.01f) return;

        Vector3 flatForward = GetFlatForward();
        Vector3 horizontalVel = new Vector3(v.x, 0f, v.z);
        float forwardSpeed = Mathf.Max(0f, Vector3.Dot(horizontalVel, flatForward));

        float authority = GetPullUpAuthority01(forwardSpeed) * noseUp01;
        if (authority <= 0f) return;

        Vector3 desiredDir = transform.forward.normalized;

        float speed01 = Mathf.Clamp01(speed / (maxForwardSpeed * 1.2f));
        float k = pitchUpRedirectStrength * authority * speed01;

        Vector3 newDir = Vector3.Slerp(v.normalized, desiredDir, 1f - Mathf.Exp(-k * Time.fixedDeltaTime));
        Vector3 newV = newDir * speed;

        newV.y = Mathf.Clamp(newV.y, -maxDownSpeed, maxRedirectUpSpeed);
        rb.linearVelocity = newV;
    }

    void ApplyBetterGravity()
    {
        float extraGravity = rb.linearVelocity.y < 0f ? extraFallGravity : extraRiseGravity;
        rb.AddForce(Vector3.down * extraGravity, ForceMode.Acceleration);
    }

    void ApplyGlideForces()
    {
        Vector3 vel = rb.linearVelocity;
        Vector3 flatForward = GetFlatForward();
        Vector3 horizontalVel = new Vector3(vel.x, 0f, vel.z);

        float forwardSpeed = Mathf.Max(0f, Vector3.Dot(horizontalVel, flatForward));
        float speed01 = Mathf.Clamp01(forwardSpeed / Mathf.Max(0.01f, maxForwardSpeed));
        float speedFactor = Mathf.Pow(speed01, speedLiftPower);

        float noseUp01 = GetNoseUp01();
        float noseUpFactor = Mathf.Pow(noseUp01, noseUpPower);

        float authority = GetPullUpAuthority01(forwardSpeed);

        float bank01 = Mathf.Clamp01(Mathf.Abs(targetRoll) / rollLimit);
        float bankMult = 1f - (bank01 * bankLiftReduction);

        float liftAccel = noseUpLift * speedFactor * noseUpFactor * authority * bankMult;
        liftAccel = Mathf.Clamp(liftAccel, 0f, maxLift);
        rb.AddForce(Vector3.up * liftAccel, ForceMode.Acceleration);

        float forwardDragAccel = pullUpForwardDrag * speedFactor * noseUpFactor * Mathf.Lerp(0.35f, 1f, authority);
        rb.AddForce(-flatForward * forwardDragAccel, ForceMode.Acceleration);

        float stallSink01 = Mathf.Clamp01((stallSinkStartSpeed - forwardSpeed) / Mathf.Max(0.01f, stallSinkStartSpeed));
        float stallSinkAccel = stallSinkForce * stallSink01 * noseUpFactor;
        rb.AddForce(Vector3.down * stallSinkAccel, ForceMode.Acceleration);

        float sinkAccel = turnSinkForce * bank01 * speed01;
        rb.AddForce(Vector3.down * sinkAccel, ForceMode.Acceleration);

        float glideFactor = Mathf.InverseLerp(pitchLimit, -pitchLimit, targetPitch);
        rb.linearDamping = Mathf.Lerp(baseLinearDamping, glideLinearDamping, glideFactor);
    }

    void ClampVelocity()
    {
        Vector3 v = rb.linearVelocity;
        v.y = Mathf.Clamp(v.y, -maxDownSpeed, maxUpSpeed);

        Vector3 flatForward = GetFlatForward();
        Vector3 horizontalVel = new Vector3(v.x, 0f, v.z);
        float forwardComponent = Vector3.Dot(horizontalVel, flatForward);

        if (forwardComponent > maxForwardSpeed)
        {
            float excess = forwardComponent - maxForwardSpeed;
            horizontalVel -= flatForward * excess;
            v.x = horizontalVel.x;
            v.z = horizontalVel.z;
        }

        rb.linearVelocity = v;
    }

    float GetPullUpAuthority01(float forwardSpeed)
    {
        float t = Mathf.InverseLerp(stallStartSpeed, stallSpeed, forwardSpeed);
        return Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t));
    }

    float GetForwardSpeed01()
    {
        Vector3 flatForward = GetFlatForward();
        Vector3 v = rb.linearVelocity;
        Vector3 horizontalVel = new Vector3(v.x, 0f, v.z);

        float forwardSpeed = Vector3.Dot(horizontalVel, flatForward);
        forwardSpeed = Mathf.Max(0f, forwardSpeed);
        return Mathf.Clamp01(forwardSpeed / maxForwardSpeed);
    }

    float GetNoseUp01()
    {
        float noseUpAngle = Mathf.Max(0f, -targetPitch - noseUpDeadzoneDegrees);
        return Mathf.Clamp01(noseUpAngle / Mathf.Max(0.01f, (pitchLimit - noseUpDeadzoneDegrees)));
    }

    Vector3 GetFlatForward()
    {
        Vector3 flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
        if (flatForward.sqrMagnitude < 0.0001f)
            flatForward = Vector3.forward;
        return flatForward.normalized;
    }

    float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle > 180f) angle -= 360f;
        return angle;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            HandleDeath();
        }
        else if (other.CompareTag("Collectable"))
        {
            Destroy(other.gameObject);
        }
    }

    void HandleDeath()
    {
        if (isDead) return;
        isDead = true;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.EndGame();
        }
    }
}