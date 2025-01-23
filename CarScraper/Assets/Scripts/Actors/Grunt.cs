using CarScraper.Systems.ServiceLocator;
using UnityEngine;

namespace CarScraper.Actors
{
    public class Grunt : MonoBehaviour, IEnemy
    {
        [Header("References")]
        [SerializeField] private EnemyBrain brain;
        [SerializeField] private BulletPool bulletPool;

        [Header("Detection")]
        [SerializeField] private float detectionRange;
        [SerializeField] private bool seesPlayer;

        [Header("Attack")]
        [SerializeField] private float shootTimer;
        [SerializeField] private float shootCooldown = 0.5f;


        private void Start()
        {
            // Get components
            bulletPool = GetComponent<BulletPool>();

            // Set variables
            shootTimer = shootCooldown;

            // Get the Enemy Brain as a service
            brain = ServiceLocator.ForSceneOf(this).Get<EnemyBrain>();

            // Register this as an Enemy to be tracked
            brain.Register(this);
        }

        public void TickUpdate(float time, float delta)
        {
            // Detect Player
            (bool canShootPlayer, Vector3 directionToPlayer) = CanShootPlayer();

            // Exit case - if the Player cannot be shot
            if (!canShootPlayer) return;

            // Check if the shoot timer is running
            if (shootTimer > 0f)
            {
                // Decrement the shoot timer by delta
                shootTimer -= delta;
                return;
            }

            // Reset the shoot timer
            shootTimer = shootCooldown;

            // Get the bullet pool
            bulletPool.Pool.Get().Shoot(directionToPlayer);
        }

        /// <summary>
        /// Check if the Enemy can shoot the Player
        /// </summary>
        private (bool canSeePlayer, Vector3 directionToPlayer) CanShootPlayer()
        {
            // Get the Player and Enemy positions
            Vector3 playerPosition = brain.Player.position;
            Vector3 enemyPosition = transform.position;

            // Get the distance from between the Player and the Enemy
            float distanceToPlayer = Vector3.Distance(playerPosition, enemyPosition);

            // Exit case - if the distance between the Enemy and the Player is greater than the detection range
            if (distanceToPlayer > detectionRange) return (false, Vector3.zero);

            // Get the direction to the Player from the Enemy
            Vector3 directionToPlayer = (playerPosition - enemyPosition).normalized;

            // Check if an obstacle is in between the two
            bool hitObstacle = Physics.Raycast(enemyPosition, directionToPlayer, distanceToPlayer, brain.ObstacleLayers);

            // Debug
            Debug.DrawRay(enemyPosition, directionToPlayer * distanceToPlayer, Color.red);

            // Rotate the Enemy to face the Player
            if (!hitObstacle)
            {
                // Rotate the Enemy to face the Player
                Vector3 flatDirection = new Vector3(directionToPlayer.x, 0, directionToPlayer.z);

                // Check for unnecessary calculations if the direction is near zero
                if (flatDirection.sqrMagnitude > 0.01f) 
                {
                    // Rotate towards the player
                    Quaternion lookRotation = Quaternion.LookRotation(flatDirection, Vector3.up);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
                }
            }

            return (!hitObstacle, directionToPlayer);
        }
    }
}
