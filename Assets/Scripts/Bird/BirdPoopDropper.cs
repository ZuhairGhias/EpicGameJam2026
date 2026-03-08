using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BirdPoopDropper : MonoBehaviour
{
    [Header("References")]
    public Rigidbody poopPrefab;
    public Transform spawnPoint;
    public LineRenderer line;
    public AudioClip dropSfx;

    [Header("Trajectory / Feel")]
    public float extraDownSpeed = 0f;     // laxatives can raise this (e.g. 8�15)
    public float inheritMultiplier = 1f;  // 1 = full inherit
    public int segments = 25;
    public float timeStep = 0.06f;

    [Header("Preview collision")]
    public LayerMask previewCollideMask;

    private BirdGlideController bird;
    private Collider[] playerColliders;
    private AudioSource dropAudioSource;

    void Awake()
    {
        bird = GetComponent<BirdGlideController>();
        playerColliders = GetComponentsInChildren<Collider>();
        dropAudioSource = GetComponent<AudioSource>();

        if (line != null)
        {
            line.enabled = true;
            line.useWorldSpace = true;
            line.positionCount = segments;
        }
    }

    void Update()
    {
        if (line != null)
            DrawTrajectory();

        if (Input.GetMouseButtonDown(0))
            Drop();
    }

    Vector3 GetInitialVelocity()
    {
        Vector3 v = Vector3.zero;
        if (bird != null) v = bird.CurrentVelocity * inheritMultiplier;

        return v + Vector3.down * extraDownSpeed;
    }

    void Drop()
    {
        if (poopPrefab == null || spawnPoint == null) return;

        Rigidbody rb = Instantiate(poopPrefab, spawnPoint.position, Quaternion.identity);

        // Unity 6+: use linearVelocity
        rb.linearVelocity = GetInitialVelocity();

        // Ignore collisions with the player (prevents instant self-hit)
        Collider poopCol = rb.GetComponent<Collider>();
        if (poopCol != null)
        {
            foreach (var c in playerColliders)
            {
                if (c != null) Physics.IgnoreCollision(poopCol, c);
            }
        }

        if (dropAudioSource != null && dropSfx != null)
            dropAudioSource.PlayOneShot(dropSfx);
    }

    void DrawTrajectory()
    {
        if (spawnPoint == null) return;

        if (line.positionCount != segments)
            line.positionCount = segments;

        Vector3 p0 = spawnPoint.position;
        Vector3 v0 = GetInitialVelocity();
        Vector3 g = Physics.gravity;

        Vector3 prev = p0;

        for (int i = 0; i < segments; i++)
        {
            float t = i * timeStep;
            Vector3 p = p0 + v0 * t + 0.5f * g * t * t;

            if (i > 0)
            {
                Vector3 dir = p - prev;
                float dist = dir.magnitude;

                if (dist > 0.0001f &&
                    Physics.Raycast(prev, dir.normalized, out RaycastHit hit, dist, previewCollideMask))
                {
                    p = hit.point;
                    line.SetPosition(i, p);
                    for (int j = i + 1; j < segments; j++) line.SetPosition(j, p);
                    return;
                }
            }

            line.SetPosition(i, p);
            prev = p;
        }
    }
}
