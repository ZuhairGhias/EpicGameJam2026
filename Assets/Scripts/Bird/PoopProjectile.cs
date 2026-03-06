using UnityEngine;

public class PoopProjectile : MonoBehaviour
{
    [Header("Lifetime")]
    public float lifeSeconds = 5f;

    [Header("Impact FX")]
    public GameObject splatterDecalPrefab;
    public ParticleSystem splatterParticlesPrefab;
    public float decalNormalOffset = 0.01f;
    public float decalLifetime = 10f;
    public bool parentDecalToHitObject = true;
    public Vector2 decalScaleRange = new Vector2(0.9f, 1.2f);

    private bool hasImpacted;

    void Start()
    {
        Destroy(gameObject, lifeSeconds);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Prevent duplicate impact logic when multiple contacts fire in one frame.
        if (hasImpacted) return;
        hasImpacted = true;

        ContactPoint cp = collision.GetContact(0);

        // Find a Target on the thing we hit (or its parents)
        Target t = collision.collider.GetComponentInParent<Target>();
        if (t != null)
        {
            t.Hit(cp.point, cp.normal);
        }

        SpawnImpactFx(collision.collider, cp.point, cp.normal);

        // Destroy projectile on impact
        Destroy(gameObject);
    }

    void SpawnImpactFx(Collider hitCollider, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (splatterParticlesPrefab != null)
        {
            Vector3 particleForward = -hitNormal;
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null && rb.linearVelocity.sqrMagnitude > 0.0001f)
                particleForward = rb.linearVelocity.normalized;

            Quaternion particleRot = Quaternion.LookRotation(particleForward);
            ParticleSystem ps = Instantiate(splatterParticlesPrefab, hitPoint, particleRot);
            ps.Play();
        }

        if (splatterDecalPrefab != null)
        {
            Quaternion decalRot = Quaternion.LookRotation(hitNormal) * Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
            Vector3 decalPos = hitPoint + hitNormal * decalNormalOffset;
            Transform parent = (parentDecalToHitObject && hitCollider != null) ? hitCollider.transform : null;

            GameObject decal = Instantiate(splatterDecalPrefab, decalPos, decalRot, parent);

            float scale = Random.Range(decalScaleRange.x, decalScaleRange.y);
            decal.transform.localScale *= scale;

            if (decalLifetime > 0f)
                Destroy(decal, decalLifetime);
        }
    }
}
