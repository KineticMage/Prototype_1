using CarScraper.Systems.ServiceLocator;
using CarScraper.Vehicles;
using System.Collections.Generic;
using UnityEngine;

namespace CarScraper.Actors
{
    public class EnemyBrain : MonoBehaviour
    {
        [SerializeField] private LayerMask obstacleLayers;
        [SerializeField] private List<IEnemy> enemies;
        [SerializeField] private Transform player;

        [Header("Time")]
        [SerializeField] private float time;

        public Transform Player { get => player; }
        public Rigidbody PlayerRB { get; private set; }

        public LayerMask ObstacleLayers { get => obstacleLayers; }

        private void Awake()
        {
            // Initialize the list
            enemies = new List<IEnemy>();

            // Register this as a service
            ServiceLocator.ForSceneOf(this).Register(this);
        }

        private void Update()
        {
            // Get the deltaTime and increment the total Time
            float delta = Time.deltaTime;
            time += delta;

            // Iterate through each enemy
            foreach (IEnemy enemy in enemies)
            {
                // Update the enemy
                enemy.TickUpdate(time, delta);
            }
        }

        /// <summary>
        /// Register an Enemy to be managed
        /// </summary>
        public void Register(IEnemy enemy)
        {
            // Exit case - the Enemy is already being managed
            if (enemies.Contains(enemy)) return;

            // Add the Enemy to the list
            enemies.Add(enemy);
        }

        /// <summary>
        /// Deregister an Enemy from being managed
        /// </summary>
        public void Deregister(IEnemy enemy)
        {
            // Ext case - the Enemy is not being managed
            if (!enemies.Contains(enemy)) return;

            // Remove the Enemy from the list
            enemies.Remove(enemy);
        }

        /// <summary>
        /// Register the Player
        /// </summary>
        public void RegisterPlayer(Transform player)
        {
            this.player = player;
            PlayerRB = player.parent.GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Deregister the Player
        /// </summary>
        /// <param name="player"></param>
        public void DeregisterPlayer(Transform player)
        {
            // Exit case - this player is not registered
            if (this.player != player) return;

            player = null;
            PlayerRB = null;
        }
    }
}
