using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

namespace CarScraper.Actors
{
    public class Hazard : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject explosionEffect;
        private Rigidbody rb;
        private BoxCollider boxCollider;
        private ObjectPool<Hazard> pool;

        [SerializeField] private LayerMask collisionLayers;
        [SerializeField] private LayerMask activateLayers;
        [SerializeField] private float damage;
        [SerializeField] private bool primed;
        private Coroutine explodeCoroutine;

        public Action OnExplode = delegate { };

        public void Initialize(ObjectPool<Hazard> pool)
        {
            // Get componnets
            rb = GetComponent<Rigidbody>();
            boxCollider = GetComponent<BoxCollider>();
            
            // Assign variables
            this.pool = pool;
        }

        /// <summary>
        /// Reset the Hazard
        /// </summary>
        public void ResetHazard()
        {
            // Don't prime the object
            primed = false;

            // Add stronger gravity
            rb.AddForce(Physics.gravity * 10f);
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Exit case - already primed
            if (primed) return;

            // Exit case - the collider is not on one of the collision layers
            if ((collisionLayers.value & (1 << collision.gameObject.layer)) <= 0)
            {
                // Check if the collider is an activate layer
                if ((activateLayers.value & (1 << collision.gameObject.layer)) > 0)
                {
                    // Prime the Hazard
                    Prime();
                }

                return;
            }

            // Attempt to get a damageable component from the collider
            IDamageable damageable = collision.gameObject.GetComponentInChildren<IDamageable>();

            // Exit case - the collider is not Damageable
            if (damageable == null) return;

            // Explode the Hazard
            Explode();
        }

        private void Prime()
        {
            // Check if the explosion coroutine is running
            if (explodeCoroutine != null)
                // Stop the coroutine
                StopCoroutine(explodeCoroutine);

            // Set primed
            primed = true;

            // Start the explosion coroutine
            explodeCoroutine = StartCoroutine(ExplosionTimer());
        }

        /// <summary>
        /// Coroutine to handle waiting for the explosion
        /// </summary>
        private IEnumerator ExplosionTimer()
        {
            // Wait for two seconds
            yield return new WaitForSeconds(2f);

            // Explode the Hazard
            Explode();
        }

        /// <summary>
        /// Explode the Hazard and damage all Damageables within the radius
        /// </summary>
        private void Explode()
        {
            // Set primed
            primed = true;

            // Instantiate the boom effect
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

            // Define the explosion radius
            float explosionRadius = boxCollider.bounds.extents.x * 5f;

            // Check collisions detected by a sphere
            Collider[] collisions = Physics.OverlapSphere(transform.position, explosionRadius, collisionLayers);

            // Iterate through each collision
            foreach (Collider collision in collisions)
            {
                // Skip if the collider does not have a damagebale component
                if (!collision.TryGetComponent(out IDamageable damageable)) continue;

                // Damage the collision object
                damageable.Damage(damage);

                rb.AddExplosionForce(damage, transform.position, explosionRadius);
            }

            // Call the explode action
            OnExplode.Invoke();

            // Destroy the Hazard
            pool.Release(this);
        }
    }
}
