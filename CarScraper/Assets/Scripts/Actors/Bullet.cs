using UnityEngine;
using UnityEngine.Pool;

namespace CarScraper.Actors
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private LayerMask collisionLayers;
        [SerializeField] private float speed;
        [SerializeField] private float damage;

        [SerializeField] private float currentTime;
        [SerializeField] private float destructTime;

        private Grunt enemy;
        private ObjectPool<Bullet> pool;

        private Rigidbody rb;

        private void Update()
        {
            // Update the current time
            currentTime += Time.deltaTime;

            // Exit case - the current time is less than the destruct time
            if(currentTime < destructTime) return;

            // Return the Bullet to the pool
            pool.Release(this);
        }

        /// <summary>
        /// Initialize the Bullet
        /// </summary>
        public void Initialize(ObjectPool<Bullet> pool, Grunt enemy)
        {
            // Set references
            this.pool = pool;
            this.enemy = enemy;

            // Set variables
            currentTime = 0f;

            // Get the Rigidbody component
            rb = GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Shoot the Bullet
        /// </summary>
        public void Shoot(Vector3 direction) => rb.AddForce(direction * speed, ForceMode.Impulse);

        private void OnTriggerEnter(Collider collision)
        {
            // Exit case - the collider is not on one of the collision layers
            if ((collisionLayers.value & (1 << collision.gameObject.layer)) <= 0) return;

            // Exit case - the colliding object is the enemy that is shooting
            if (collision.gameObject == enemy.gameObject) return;

            // Exit case - the collider is not Damageable
            if (!collision.gameObject.TryGetComponent(out IDamageable damageable)) return;

            // Damage the collider
            damageable.Damage(damage);

            // Return the Bullet to the pool
            pool.Release(this);
        }
    }
}
