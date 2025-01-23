using UnityEngine;
using UnityEngine.Pool;

namespace CarScraper.Actors
{
    public class HazardPool : MonoBehaviour
    {
        private Boss boss;
        private ObjectPool<Hazard> pool;
        [SerializeField] private Hazard hazardPrefab;

        public ObjectPool<Hazard> Pool { get => pool; }

        private void Awake()
        {
            // Get components
            boss = GetComponent<Boss>();

            // Instantiate the Object Pool
            pool = new ObjectPool<Hazard>(CreateHazard, OnTakeHazardFromPool, OnReturnHazardToPool, OnDestroyHazard, true, 100, 1000);
        }

        /// <summary>
        /// Create a Hazard for the Object Pool
        /// </summary>
        private Hazard CreateHazard()
        {
            // Instantiate the Bullet
            Hazard hazard = Instantiate(hazardPrefab, boss.GetHazardSpawnPosition(), Quaternion.identity);

            return hazard;
        }

        /// <summary>
        /// Take a Hazard from the Object Pool
        /// </summary>
        private void OnTakeHazardFromPool(Hazard hazard)
        {
            // Set the transform and rotation
            hazard.transform.position = boss.GetHazardSpawnPosition();
            hazard.transform.rotation = Quaternion.identity;

            // Activate the Bullet
            hazard.gameObject.SetActive(true);
        }

        /// <summary>
        /// Return a Hazard to the Object Pool
        /// </summary>
        private void OnReturnHazardToPool(Hazard hazard)
        {
            // Deactivate the Hazard
            hazard.gameObject.SetActive(false);
        }

        /// <summary>
        /// Handle the destruction of a Hazard in cases of Object Pool overflow
        /// </summary>
        private void OnDestroyHazard(Hazard hazard)
        {
            // Destroy the bullet
            Destroy(hazard.gameObject);
        }
    }
}
