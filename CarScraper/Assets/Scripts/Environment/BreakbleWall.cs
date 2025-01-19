using UnityEngine;

namespace CarScraper
{
    public class BreakbleWall : MonoBehaviour
    {
        public GameObject wholeWall;  // The intact wall object
        public GameObject brokenWall; // The broken wall object

        private void Start()
        {
            // Ensure the correct initial state
            wholeWall.SetActive(true);
            brokenWall.SetActive(false);
        }
        private void OnTriggerEnter(Collider other)
        {
           
            if (other.transform.root.tag == "Player" || other.transform.root.tag == "Drone")
            {
                BreakWall();
            }
        }
      

        private void BreakWall()
        {
            wholeWall.SetActive(false);   // Hide the whole wall
            brokenWall.SetActive(true);   // Show the broken wall
        }
    }
}
