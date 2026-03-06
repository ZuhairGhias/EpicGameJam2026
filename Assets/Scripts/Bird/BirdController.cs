using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BirdController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float turnSpeed = 120f; // degrees per second

    [Header("Optional")]
    public float acceleration = 5f;
    public float maxSpeed = 10f;

    // Expose this for the poop dropper
    public Vector3 CurrentVelocity { get; private set; }

    private float currentSpeed;
    private float turnInput;

    private Rigidbody rb;
    private Vector3 lastPos;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Prevent tipping from collisions
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        lastPos = rb.position;
    }

    void Start()
    {
        currentSpeed = moveSpeed;
    }

    void Update()
    {
        // Read input in Update
        turnInput = 0f;
        if (Input.GetKey(KeyCode.LeftArrow)) turnInput = -1f;
        if (Input.GetKey(KeyCode.RightArrow)) turnInput = 1f;
    }

    void FixedUpdate()
    {
        currentSpeed = moveSpeed;

        float turnDegrees = turnInput * turnSpeed * Time.fixedDeltaTime;
        Quaternion deltaRot = Quaternion.Euler(0f, turnDegrees, 0f);
        rb.MoveRotation(rb.rotation * deltaRot);

        Vector3 forward = rb.rotation * Vector3.forward;
        rb.MovePosition(rb.position + forward * currentSpeed * Time.fixedDeltaTime);

        CurrentVelocity = (rb.position - lastPos) / Time.fixedDeltaTime;
        lastPos = rb.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Obstacle")
        {
            ResetGame();
        }
    }

    void ResetGame()
    {
        transform.position = new Vector3(0f, 4.25f, 0f);
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }
}