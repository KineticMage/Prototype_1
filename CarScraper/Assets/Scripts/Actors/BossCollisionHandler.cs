using CarScraper.Actors;
using UnityEngine;

namespace CarScraper
{
    public class BossCollisionHandler : MonoBehaviour, IDamageable
    {
        private Rigidbody rb;

        [Header("Fields")]
        [SerializeField] private float health = 25f;

        private void OnCollisionEnter(Collision collision)
        {
            // Exit case - the colliding object is not a vehicle
            if (collision.gameObject.tag != "Draggable") return;

            // Exit case - the enemy or the vehicle does not have a Rigidbody
            if (rb == null) return;

            // Damage the Enemy
            Damage(5f);

            // Destroy the collision object
            Destroy(collision.gameObject);
        }

        /// <summary>
        /// Damage the Enemy and check for ragdolling
        /// </summary>
        public void Damage(float damage)
        {
            // Subtract the health by the damage
            health -= damage;

            // If at 0 health or below, destroy the Enemy
            if (health <= 0) Destroy(this);
        }
    }
}
