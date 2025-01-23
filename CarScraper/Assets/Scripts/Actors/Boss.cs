using CarScraper.Systems.ServiceLocator;
using UnityEngine;

namespace CarScraper.Actors
{
    public class Boss : MonoBehaviour, IEnemy
    {
        [Header("References")]
        [SerializeField] private EnemyBrain brain;
        [SerializeField] private HazardPool hazardPool;
        [SerializeField] private CapsuleCollider capsuleCollider;
        private BossAttackFeedback attackFeedback;

        [Header("Attack")]
        [SerializeField] private float range;
        [SerializeField] private float attackTimer;
        [SerializeField] private float attackCooldown = 2f;
        private Vector3 detectBasePosition;
        private Vector3 feedbackSpawnPosition;
        private Vector3 hazardSpawnPosition;

        private void OnValidate()
        {
            // Check if the capsule collider exists
            if (capsuleCollider == null)
                // If not, retrieve it
                capsuleCollider = GetComponent<CapsuleCollider>();

            // Set the base position
            detectBasePosition = transform.position - Vector3.up * (capsuleCollider.height / 2);
        }

        private void OnDestroy()
        {
            // Deregister this as an Enemy
            brain.Deregister(this);
        }

        private void Start()
        {
            // Get components
            hazardPool = GetComponent<HazardPool>();
            capsuleCollider = GetComponent<CapsuleCollider>();
            attackFeedback = GetComponentInChildren<BossAttackFeedback>();

            // Check if the capsule collider exists
            if (capsuleCollider != null)
                // If so, set the base position
                detectBasePosition = transform.position - Vector3.up * (capsuleCollider.height / 2);

            // Set variables
            attackTimer = attackCooldown;

            // Get the Enemy Brain as a service
            brain = ServiceLocator.ForSceneOf(this).Get<EnemyBrain>();

            // Register this as an Enemy to be tracked
            brain.Register(this);
        }

        public void TickUpdate(float time, float delta)
        {
            // Exit case - player data is not set within the brain
            if (brain.Player == null || brain.PlayerRB == null) return;

            // Exit case - the Player is out of range
            if (Vector3.Distance(brain.Player.transform.position, transform.position) > range) return;

            // Exit case - the player does not have a Rigidbody
            if (brain.PlayerRB == null) return;

            // Exit case - the attack is still on cooldown
            if(attackTimer < attackCooldown)
            {
                attackTimer += delta;
                return;
            }

            // Predict the player's future position
            Vector3 playerVelocity = brain.PlayerRB.linearVelocity;
            Vector3 playerFuturePosition = brain.Player.transform.position + playerVelocity;

            // Set spawn positions
            feedbackSpawnPosition = new Vector3(playerFuturePosition.x, 0.01f, playerFuturePosition.z);
            hazardSpawnPosition = new Vector3(playerFuturePosition.x, playerFuturePosition.y + 50f, playerFuturePosition.z);

            // Spanw a hazard
            Hazard hazardToSpawn = hazardPool.Pool.Get();

            // Activate the attack feedback
            attackFeedback.Activate(feedbackSpawnPosition, hazardToSpawn);

            // Reset the attack timer
            attackTimer = 0f;
        }

        /// <summary>
        /// Get the current Hazard spawn position
        /// </summary>
        public Vector3 GetHazardSpawnPosition() => hazardSpawnPosition;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(detectBasePosition, range);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(new Vector3(hazardSpawnPosition.x, hazardSpawnPosition.y - 50f, hazardSpawnPosition.z), 0.5f);
        }
    }
}
