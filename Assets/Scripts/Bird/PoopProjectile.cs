using UnityEngine;

public class PoopProjectile : MonoBehaviour
{
    public float lifeSeconds = 5f;

    void Start()
    {
        Destroy(gameObject, lifeSeconds);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Find a Target on the thing we hit (or its parents)
        Target t = collision.collider.GetComponentInParent<Target>();
        if (t != null)
        {
            ContactPoint cp = collision.GetContact(0);
            t.Hit(cp.point, cp.normal);
        }

        // Destroy projectile on impact
        Destroy(gameObject);
    }
}