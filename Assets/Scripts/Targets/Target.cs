using UnityEngine;

public class Target : MonoBehaviour
{
    public enum TargetType
    {
        Pedestrian,
        Car,
        Biker,
        Police,
        Duck
    }

    [Header("Target Settings")]
    public TargetType targetType = TargetType.Pedestrian;
    public int points = 0;

    [Header("Hit Reaction")]
    public bool destroyOnHit = false;
    public float disableSeconds = 0.5f; // brief "stun" so you can't spam one target instantly
    public GameObject hitVfxPrefab;     // optional
    public AudioClip hitSfx;            // optional

    [Header("Debug")]
    public bool logHits = false;

    private bool canBeHit = true;

    // Call this from projectile or raycast hit logic
    public void Hit(Vector3 hitPoint, Vector3 hitNormal)
    {
        if (!canBeHit) return;

        canBeHit = false;

        if (logHits) Debug.Log($"Hit {name} ({targetType}) for {points} points");

        // Spawn VFX
        if (hitVfxPrefab != null)
        {
            Quaternion rot = (hitNormal.sqrMagnitude > 0.001f)
                ? Quaternion.LookRotation(hitNormal)
                : Quaternion.identity;

            Instantiate(hitVfxPrefab, hitPoint, rot);
        }

        // Play SFX (simple: play at point)
        if (hitSfx != null)
        {
            AudioSource.PlayClipAtPoint(hitSfx, hitPoint);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterHit(this, this.transform.position);
        }
        else
        {
            Debug.LogError("GameManager.Instance is null. Add a GameManager to the scene.");
        }

        // Optional: destroy or temporarily disable collision
        if (destroyOnHit)
        {
            Destroy(gameObject);
        }
        else
        {
            // Briefly disable collider so it can't be hit repeatedly in the same frame burst
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;

            Invoke(nameof(ResetHit), disableSeconds);
        }
    }

    private void ResetHit()
    {
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = true;

        canBeHit = true;
    }
}