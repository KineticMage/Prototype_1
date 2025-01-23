using UnityEngine;
using UnityEngine.Pool;

namespace CarScraper.Actors
{
    public class Hazard : MonoBehaviour
    {
        [SerializeField] private LayerMask collisionLayers;
        [SerializeField] private float damage;
        private ObjectPool<Hazard> pool;

        private Rigidbody rb;

        /// <summary>
        /// Initialize the Bullet
        /// </summary>
        public void Initialize(ObjectPool<Hazard> pool)
        {
            // Set references
            this.pool = pool;

            // Get the Rigidbody component
            rb = GetComponent<Rigidbody>();

            // Add stronger gravity to the Hazard
            rb.AddForce(Physics.gravity * rb.mass);
        }

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
