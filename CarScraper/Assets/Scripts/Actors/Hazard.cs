using UnityEngine;
using UnityEngine.Pool;

namespace CarScraper.Actors
{
    public class Hazard : MonoBehaviour
    {
        [SerializeField] private LayerMask collisionLayers;
        [SerializeField] private float damage;

        private void OnCollisionEnter(Collision collision)
        {
            // Exit case - the collider is not on one of the collision layers
            if ((collisionLayers.value & (1 << collision.gameObject.layer)) <= 0) return;

            // Exit case - the collider is not Damageable
            if (!collision.gameObject.TryGetComponent(out IDamageable damageable)) return;

            // Damage the collider
            damageable.Damage(damage);
        }
    }
}
