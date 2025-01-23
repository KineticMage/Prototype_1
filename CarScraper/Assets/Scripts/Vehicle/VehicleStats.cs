using CarScraper.Actors;
using CarScraper.Systems.ServiceLocator;
using UnityEngine;

namespace CarScraper
{
    public class VehicleStats : MonoBehaviour, IDamageable
    {
        [SerializeField] private EnemyBrain enemyBrain;
        [SerializeField] private float health;
        [SerializeField] GameObject GameLooseUI;

        private void Start()
        {
            // Register this as the Player for the Enemy Brain
            enemyBrain = ServiceLocator.ForSceneOf(this).Get<EnemyBrain>();
            enemyBrain.RegisterPlayer(transform);
        }

        /// <summary>
        /// Take damage for the vehicle
        /// </summary>
        public void Damage(float damage)
        {
            // Subtract the health by the damage taken
            health -= damage;

            // Check lose conditions
            if(health <= 0)
            {
                // Deregister the Player from the Enemy Brain
                enemyBrain.DeregisterPlayer(transform);

                // Destroy the car
                Destroy(transform.parent.gameObject);
            }
        }

        private void Update()
        {
            if (health == 0) 
            { 
                GameLooseUI.SetActive(true);
            }
        }
    }
}
