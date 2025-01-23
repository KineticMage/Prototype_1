using CarScraper.Vehicles;
using UnityEngine;

namespace CarScraper.Actors
{
    public class EnemyCollisionHandler : MonoBehaviour, IDamageable
    {
        private Rigidbody rb;

        [Header("Fields")]
        [SerializeField] private float health = 10f;
        [SerializeField] private float forceMultiplier = 10f; // Adjust to control the overall force applied
        [SerializeField] private float upwardForceFactor = 0.5f; // Controls how much upward force is added
        [SerializeField] private float minForce = 100f;
        [SerializeField] private float maxForce = 250f;

        private void Awake()
        {
            // Get components
            rb = GetComponent<Rigidbody>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Exit case - the colliding object is not a vehicle
            if (!collision.gameObject.TryGetComponent(out CarControls vehicle)) return;

            // Exit case - the enemy or the vehicle does not have a Rigidbody
            if (rb == null || vehicle.rigid == null) return;

            // Get the current vehicle speed
            float vehicleSpeed = vehicle.rigid.linearVelocity.magnitude;

            // Calculate the collision point and direction
            Vector3 collisionPoint = collision.contacts[0].point;
            Vector3 collisionNormal = collision.contacts[0].normal;

            // Calculate the force direction
            Vector3 forceDirection = (collisionPoint - transform.position).normalized;
            forceDirection += Vector3.up * upwardForceFactor;
            forceDirection = forceDirection.normalized;

            // Calculate the total force to apply
            Vector3 forceToApply = forceMultiplier * vehicleSpeed * forceDirection;

            // Check if the force is greater than the minimum allowed force
            if (forceToApply.magnitude < minForce)
                // Clamp the force
                forceToApply = forceToApply.normalized * minForce;
            // Check if the force is greater than the maximum allowed force
            else if (forceToApply.magnitude > maxForce)
                // Clamp the force
                forceToApply = forceToApply.normalized * maxForce;

            // Normalize the force between 0 - 1
            float normalizedForce = (forceToApply.magnitude - minForce) / (maxForce - minForce);

            // Lerp the value within a range of 1 - 10 for the final damage
            float damage = Mathf.Lerp(1, 10, normalizedForce);

            // Damage the Enemy
            Damage(damage);

            // Apply the force to the enemy
            rb.AddForce(forceToApply, ForceMode.Impulse);
        }

        /// <summary>
        /// Damage the Enemy and check for ragdolling
        /// </summary>
        public void Damage(float damage)
        {
            // Subtract the health by the damage
            health -= damage;

            // If at 0 health or below, remove all constraints to allow for ragdoll moments
            if (health <= 0) rb.constraints = RigidbodyConstraints.None;
        }
    }
}
