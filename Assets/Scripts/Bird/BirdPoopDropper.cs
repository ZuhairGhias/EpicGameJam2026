using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BirdPoopDropper : MonoBehaviour
{
    [Header("References")]
    public Rigidbody poopPrefab;
    public Transform spawnPoint;
    public LineRenderer line;
    public Transform landingMarker;
    public AudioClip dropSfx;

    [Header("Trajectory / Feel")]
    public float extraDownSpeed = 0f;     // laxatives can raise this (e.g. 8-15)
    public float inheritMultiplier = 1f;  // 1 = full inherit
    public float dropCooldownSeconds = 0.5f;
    public int segments = 25;
    public float timeStep = 0.06f;

    [Header("Preview collision")]
    public bool showTrajectoryPreview = true;
    public bool showLandingMarker = true;
    public LayerMask previewCollideMask;
    public float landingMarkerNormalOffset = 0.02f;
    public LayerMask landingMarkerGroundMask = ~0;
    public float landingMarkerGroundProbeHeight = 30f;
    public float landingMarkerGroundProbeDistance = 100f;

    private BirdGlideController bird;
    private Collider[] playerColliders;
    private AudioSource dropAudioSource;
    private float nextAllowedDropTime;

    void Awake()
    {
        bird = GetComponent<BirdGlideController>();
        playerColliders = GetComponentsInChildren<Collider>();
        dropAudioSource = GetComponent<AudioSource>();

        if (line != null)
        {
            line.enabled = showTrajectoryPreview;
            line.useWorldSpace = true;
            line.positionCount = segments;
        }

        if (landingMarker != null)
            landingMarker.gameObject.SetActive(false);
    }

    void Update()
    {
        if (line != null)
        {
            line.enabled = showTrajectoryPreview;

            if (showTrajectoryPreview)
                DrawTrajectory();
            else if (landingMarker != null && landingMarker.gameObject.activeSelf)
                landingMarker.gameObject.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= nextAllowedDropTime)
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
        nextAllowedDropTime = Time.time + dropCooldownSeconds;

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
        bool foundHit = false;
        Vector3 hitPoint = Vector3.zero;
        Vector3 hitNormal = Vector3.up;

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

                    foundHit = true;
                    hitPoint = hit.point;
                    hitNormal = hit.normal;
                    UpdateLandingMarker(foundHit, hitPoint, hitNormal);
                    return;
                }
            }

            line.SetPosition(i, p);
            prev = p;
        }

        UpdateLandingMarker(foundHit, hitPoint, hitNormal);
    }

    void UpdateLandingMarker(bool foundHit, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (landingMarker == null) return;

        bool shouldShow = showLandingMarker && showTrajectoryPreview && foundHit;
        if (landingMarker.gameObject.activeSelf != shouldShow)
            landingMarker.gameObject.SetActive(shouldShow);

        if (!shouldShow) return;

        Vector3 markerPoint = hitPoint;
        Vector3 markerNormal = hitNormal;

        Vector3 probeStart = hitPoint + Vector3.up * landingMarkerGroundProbeHeight;
        if (Physics.Raycast(
            probeStart,
            Vector3.down,
            out RaycastHit groundHit,
            landingMarkerGroundProbeDistance,
            landingMarkerGroundMask))
        {
            markerPoint = groundHit.point;
            markerNormal = groundHit.normal;
        }

        landingMarker.position = markerPoint + markerNormal * landingMarkerNormalOffset;
        landingMarker.rotation = Quaternion.LookRotation(-markerNormal);
    }
}

