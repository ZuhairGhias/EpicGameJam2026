using UnityEngine;

public class BirdController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float turnSpeed = 120f; // degrees per second

    [Header("Optional")]
    public float acceleration = 5f;
    public float maxSpeed = 10f;

    private float currentSpeed;

    void Start()
    {
        currentSpeed = moveSpeed;
    }

    void Update()
    {
        HandleTurning();
        HandleMovement();
    }

    void HandleTurning()
    {
        float turnInput = 0f;

        if (Input.GetKey(KeyCode.LeftArrow))
            turnInput = -1f;

        if (Input.GetKey(KeyCode.RightArrow))
            turnInput = 1f;

        // Rotate around Y axis
        transform.Rotate(Vector3.up * turnInput * turnSpeed * Time.deltaTime);
    }

    void HandleMovement()
    {
        // Optional gradual acceleration
        currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed, acceleration * Time.deltaTime);

        // Move forward in facing direction
        transform.position += transform.forward * currentSpeed * Time.deltaTime;
    }
}