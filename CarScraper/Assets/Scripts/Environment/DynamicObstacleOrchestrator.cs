using CarScraper.Environment;
using CarScraper.Systems.ServiceLocator;
using System.Collections.Generic;
using UnityEngine;

namespace CarScraper.Environment
{
    public class DynamicObstacleOrchestrator : MonoBehaviour
    {
        private List<IDynamicObstacle> dynamicObstacles;

        [Header("Time")]
        [SerializeField] private float time;

        private void Awake()
        {
            // Initialize the list
            dynamicObstacles = new List<IDynamicObstacle>();

            // Register this as a Service
            ServiceLocator.ForSceneOf(this).Register(this);
        }

        private void Update()
        {
            // Get the deltaTime and increment the total Time
            float delta = Time.deltaTime;
            time += delta;

            // Iterate through each Dynamic Obstacle
            foreach(IDynamicObstacle dynamicObstacle in dynamicObstacles)
            {
                // Tick the Dynamic Obstacle
                dynamicObstacle.TickUpdate(time, delta);
            }
        }

        private void FixedUpdate()
        {
            float delta = Time.deltaTime;

            // Iterate through each Dynamic Obstacle
            foreach(IDynamicObstacle dynamicObstacle in dynamicObstacles)
            {
                // Tick the Dynamic Obstacle
                dynamicObstacle.TickFixedUpdate(delta);
            }
        }

        /// <summary>
        /// Register a Dynamic Obstacle to the Orchestrator
        /// </summary>
        public void Register(IDynamicObstacle dynamicObstacle)
        {
            // Exit case - the List already contains the Dynamic Obstacle
            if (dynamicObstacles.Contains(dynamicObstacle)) return;

            // Add the Dynamic Obstacle to the List
            dynamicObstacles.Add(dynamicObstacle);
        }

        /// <summary>
        /// Deregister the Dynamic Obstacle
        /// </summary>
        public void Deregister(IDynamicObstacle dynamicObstacle)
        {
            // Exit case - the List does not contain the Dynamic Obstacle
            if (!dynamicObstacles.Contains(dynamicObstacle)) return;

            // Remove the Dynamic Obstacle from the List
            dynamicObstacles.Remove(dynamicObstacle);
        }
    }
}
