using CarScraper.Actors;
using CarScraper.Systems.ServiceLocator;
using UnityEngine;

namespace CarScraper
{
    public class VehicleStats : MonoBehaviour, IDamageable
    {

        private void Start()
        {
            // Register this as the Player for the Enemy Brain
            ServiceLocator.ForSceneOf(this).Get<EnemyBrain>().RegisterPlayer(transform);
        }

        public void Damage(float damage)
        {
            Debug.Log("Taking Damage!");
        }
    }
}
