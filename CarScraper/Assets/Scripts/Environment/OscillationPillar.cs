using CarScraper.Systems.ServiceLocator;
using UnityEngine;

namespace CarScraper.Environment
{
    public class OscillationPillar : MonoBehaviour, IDynamicObstacle
    {
        private enum OscillationPhase
        {
            WaitingToStart,
            MovingToMax,
            PausingAtMax,
            MovingToMin,
            PausingAtMin
        }

        private DynamicObstacleOrchestrator orchestrator;

        [Header("Configuration")]
        [SerializeField] private float minHeight = 0.5f;
        [SerializeField] private float maxHeight = 2.0f;
        [SerializeField] private float cycleTime = 2.0f;
        [SerializeField] private float startDelay = 0.0f;
        [SerializeField] private float pauseAtMinTime = 0.5f;
        [SerializeField] private float pauseAtMaxTime = 0.5f;
        [SerializeField] private bool startAtMax;

        private float elapsedTime = 0f;
        private float delayTimer = 0f;
        private float pauseTimer = 0f;

        private Vector3 originalScale;
        private Vector3 originalPosition;

        private OscillationPhase currentPhase = OscillationPhase.WaitingToStart;

        private void Awake()
        {
            // Store the original scale and position
            originalScale = transform.localScale;
            originalPosition = transform.position;

            // Initialize the delay timer
            delayTimer = startDelay;

            // Set initial height based on startAtMinHeight
            if (startAtMax)
            {
                // Set the maximum height
                SetHeight(maxHeight);

                // Set the current phase
                currentPhase = delayTimer <= 0f ? OscillationPhase.MovingToMin : OscillationPhase.WaitingToStart;
            }
            else
            {
                // Set the minimum height
                SetHeight(minHeight);

                // Set the current phase
                currentPhase = delayTimer <= 0f ? OscillationPhase.MovingToMax : OscillationPhase.WaitingToStart;
            }
        }

        private void Start()
        {
            // Retrieve the Dynamic Obstacle Orchestrator
            orchestrator = ServiceLocator.ForSceneOf(this).Get<DynamicObstacleOrchestrator>();

            // Register to the Dynamic Obstacle Orchestrator
            orchestrator.Register(this);
        }

        public void TickUpdate(float time, float delta)
        {
            // Check the current phase
            switch (currentPhase)
            {
                case OscillationPhase.WaitingToStart:
                    UpdateDelay(delta);
                    break;

                case OscillationPhase.MovingToMax:
                    Oscillate(delta, true);
                    break;

                case OscillationPhase.PausingAtMax:
                    Pause(delta, pauseAtMinTime, OscillationPhase.MovingToMin);
                    break;

                case OscillationPhase.MovingToMin:
                    Oscillate(delta, false);
                    break;

                case OscillationPhase.PausingAtMin:
                    Pause(delta, pauseAtMinTime, OscillationPhase.MovingToMax);
                    break;
            }
        }

        public void TickFixedUpdate(float delta) { /* Noop */ }

        /// <summary>
        /// Update the starting delay
        /// </summary>
        private void UpdateDelay(float delta)
        {
            // Count down the delay timer
            delayTimer -= delta;

            // Check if the delay timer has finished
            if (delayTimer <= 0f)
            {
                // Reset the elapsed time
                elapsedTime = 0f;

                // Set the current phase
                currentPhase = startAtMax ? OscillationPhase.MovingToMin : OscillationPhase.MovingToMax;
            }
        }

        /// <summary>
        /// Handle the oscillation of the pillar
        /// </summary>
        private void Oscillate(float delta, bool movingUp)
        {
            // Increment the elapsed time
            elapsedTime += delta;

            // Calcuate half of the cycle time
            float halfCycleTime = cycleTime / 2f;

            // Calculate the full t-value
            float t = elapsedTime / halfCycleTime;

            // Calculate the current height of the pillar
            float height = movingUp
                ? Mathf.Lerp(minHeight, maxHeight, t)
                : Mathf.Lerp(maxHeight, minHeight, t);

            // Check if the elapsed time has surpassed the half-cycle time
            if(elapsedTime >= halfCycleTime)
            {
                // Reset the elapsed time
                elapsedTime = 0f;

                // Set the next Oscillation Phase
                currentPhase = movingUp
                    ? OscillationPhase.PausingAtMax
                    : OscillationPhase.PausingAtMin;
            }

            // Set the height of the Oscillation Pillar
            SetHeight(height);
        }

        /// <summary>
        /// Handle oscillation pauses
        /// </summary>
        private void Pause(float delta, float pauseDuration, OscillationPhase nextPhase)
        {
            // Increment the pause timer
            pauseTimer += delta;

            // Check if the pause timer has exceeded the pause duration
            if(pauseTimer >= pauseDuration)
            {
                // Reset the pause timer
                pauseTimer = 0f;

                // Set the next Oscillation Phase
                currentPhase = nextPhase;
            }
        }

        /// <summary>
        /// Set the height of the Oscillation Pillar
        /// </summary>
        private void SetHeight(float height)
        {
            // Adjust the scale according to the height
            AdjustScale(height);

            // Adjust the position according ot the height
            AdjustPosition(height);
        }

        /// <summary>
        /// Adjust the Pillar's Transform Scale according to the current height value
        /// </summary>
        private void AdjustScale(float height)
        {
            // Retrieve the original scale
            Vector3 newScale = originalScale;

            // Scale the y-value based on the calculated height
            newScale.y = height / originalScale.y;

            // Set the new scale
            transform.localScale = newScale;
        }

        /// <summary>
        /// Adjust the Pillar's Transform Position according to the current height value
        /// </summary>
        private void AdjustPosition(float height)
        {
            // Retrieve the original position
            Vector3 newPosition = originalPosition;

            // Translate the y-value based on the calculated height
            newPosition.y += (height - originalScale.y * originalScale.y / 2);

            // Set the new position
            transform.position = newPosition;
        }
    }
}
