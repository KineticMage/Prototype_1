using CarScraper.Actors;
using CarScraper.Systems.ServiceLocator;
using UnityEngine;

namespace CarScraper
{
    public class VehicleStats : MonoBehaviour, IDamageable
    {
        [SerializeField] private float health;

        private void Start()
        {
            // Register this as the Player for the Enemy Brain
            ServiceLocator.ForSceneOf(this).Get<EnemyBrain>().RegisterPlayer(transform);
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
                // Lose the gamae
            }
        }
    }
}
