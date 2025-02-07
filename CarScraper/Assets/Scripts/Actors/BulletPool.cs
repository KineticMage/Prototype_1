using UnityEngine;
using UnityEngine.Pool;

namespace CarScraper.Actors
{
    public class BulletPool : MonoBehaviour
    {
        private Enemy enemy;
        [SerializeField] private ObjectPool<Bullet> pool;
        [SerializeField] private Bullet bulletPrefab;

        public ObjectPool<Bullet> Pool { get => pool; }

        private void Awake()
        {
            // Get components
            enemy = GetComponent<Enemy>();

            // Instantiate the Object Pool
            pool = new ObjectPool<Bullet>(CreateBullet, OnTakeBulletFromPool, OnReturnBulletFromPool, OnDestroyBullet, true, 10, 100);
        }

        /// <summary>
        /// Create a Bullet for the Object Pool
        /// </summary>
        private Bullet CreateBullet()
        {
            // Instantiate the Bullet
            Bullet bullet = Instantiate(bulletPrefab, enemy.transform.position, enemy.transform.rotation);

            // Initialize the Bullet
            bullet.Initialize(pool, enemy);

            return bullet;
        }

        /// <summary>
        /// Take a Bullet from the Object Pool
        /// </summary>
        private void OnTakeBulletFromPool(Bullet bullet)
        {
            // Set the transform and rotation
            bullet.transform.position = enemy.transform.position;
            bullet.transform.rotation = enemy.transform.rotation;

            // Activate the Bullet
            bullet.gameObject.SetActive(true);
        }

        /// <summary>
        /// Return a Bullet to the Object Pool
        /// </summary>
        private void OnReturnBulletFromPool(Bullet bullet)
        {
            // Deactivate the Bullet
            bullet.gameObject.SetActive(false);
        }

        /// <summary>
        /// Handle the destruction of a Bullet in cases of Object Pool overflow
        /// </summary>
        private void OnDestroyBullet(Bullet bullet)
        {
            // Destroy the bullet
            Destroy(bullet.gameObject);
        }
    }
}
