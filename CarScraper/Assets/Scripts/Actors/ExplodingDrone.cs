using UnityEngine;

namespace CarScraper
{
    public class ExplodingDrone : MonoBehaviour
    {
        public Transform player;
        public float detectionRange = 5f;
        public float speed = 2f;
        public float stoppingDistance = 0.5f;
        public GameObject explosionEffect;  // Assign an explosion prefab in the Inspector
        public float explosionForce = 500f;  // Force of the explosion
        public float explosionRadius = 3f;   // Radius of the explosion
        public float upwardsModifier = 1f;   // Adds an upwards lift to the explosion force

        private void Update()
        {
            if (player == null) return;

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= detectionRange && distanceToPlayer > stoppingDistance)
            {
                SeekPlayer();
            }
        }

        private void SeekPlayer()
        {
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (collision.tag != "Ground")  // If it's NOT the floor, explode
            {
                Explode(collision);
            }
        }

        private void Explode(Collider collision)
        {
            if (explosionEffect)
            {
                Instantiate(explosionEffect, transform.position, Quaternion.identity);
            }

            // Apply explosion force to the collided object if it has a Rigidbody
            Rigidbody rb = collision.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardsModifier, ForceMode.Impulse);
            }
            if (collision.GetComponent<VehicleStats>())
                collision.GetComponent<VehicleStats>().Damage(1);

            Destroy(gameObject);  // Destroy the NPC
        }
    }
}
