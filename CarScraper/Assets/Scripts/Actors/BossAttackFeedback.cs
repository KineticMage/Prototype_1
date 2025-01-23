using UnityEngine;

namespace CarScraper.Actors
{
    public class BossAttackFeedback : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        private Hazard currentHazard;

        private Color invisibleColor;

        private void Start()
        {
            // Get components
            spriteRenderer = GetComponent<SpriteRenderer>();

            // Set the initial color
            invisibleColor = spriteRenderer.color;
            invisibleColor.a = 0f;
        }

        private void Update()
        {
            // Exit case - there's no hazard
            if (currentHazard == null) return;

            // get the distance between the Hazard and the feedback
            float distance = Mathf.Abs(currentHazard.transform.position.y - transform.position.y);

            // Exit case - the hazard has reached the ground
            if (distance <= 0f)
            {
                // Set the invisible color
                spriteRenderer.color = invisibleColor;
                return;
            }

            // Calculate the opacity based on the distance
            float opacity = Mathf.Clamp(1 - (distance / 50f), 0.2f, 1f);
            Color color = spriteRenderer.color;
            color.a = opacity;
            spriteRenderer.color = color;
        }

        /// <summary>
        /// Activate the feedback
        /// </summary>
        public void Activate(Vector3 spawnPosition, Hazard currentHazard)
        {
            // Check if a current Hazard exists
            if(this.currentHazard != null)
            {
                // Unsubscribe from its events
                this.currentHazard.OnExplode -= OnHazardExplode;
            }

            // Set the current hazard
            this.currentHazard = currentHazard;
            this.currentHazard.OnExplode += OnHazardExplode;

            // Set the spawn position
            transform.position = spawnPosition;
        }

        /// <summary>
        /// Handle the Hazard explosion
        /// </summary>
        private void OnHazardExplode()
        {
            // Unsubscribe the event
            currentHazard.OnExplode -= OnHazardExplode;

            // Nullify the current hazard
            currentHazard = null;

            // Set the invisible color
            spriteRenderer.color = invisibleColor;
        }
    }
}
