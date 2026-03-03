using UnityEngine;

public class CameraFollowPreviewFriendly : MonoBehaviour
{
    private Transform target;

    private Vector3 positionOffset;
    private Quaternion rotationOffset;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("No GameObject tagged 'Player' found.");
            return;
        }

        target = player.transform;

        // Calculate initial offsets based on current placement
        positionOffset = transform.position - target.position;
        rotationOffset = Quaternion.Inverse(target.rotation) * transform.rotation;

        // Optional: unparent automatically
        transform.parent = null;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Follow position
        transform.position = target.position + positionOffset;
    }
}