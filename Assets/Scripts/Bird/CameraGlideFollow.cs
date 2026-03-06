using UnityEngine;

public class CameraGlideFollow : MonoBehaviour
{
    public float backDistance = 6f;
    public float height = 2f;
    public float sideOffset = 0f;
    public float followSmooth = 8f;

    private Transform playerTransform;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("CameraGlideFollow: No GameObject with tag 'Player' found.");
        }
    }

    void LateUpdate()
    {
        if (playerTransform == null) return;

        // Ignore roll by flattening forward onto the horizontal plane
        Vector3 flatForward = Vector3.ProjectOnPlane(playerTransform.forward, Vector3.up).normalized;

        // Fallback if forward is too vertical
        if (flatForward.sqrMagnitude < 0.001f)
        {
            flatForward = transform.forward;
            flatForward.y = 0f;
            flatForward.Normalize();
        }

        Vector3 targetPosition =
            playerTransform.position
            - flatForward * backDistance
            + Vector3.up * height
            + playerTransform.right * sideOffset;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            followSmooth * Time.deltaTime
        );

        transform.LookAt(playerTransform.position);
    }
}