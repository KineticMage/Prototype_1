using CarScraper.Vehicles;
using UnityEngine;
using UnityEngine.InputSystem.XR;

namespace CarScraper
{
    public class SpeedBoost : MonoBehaviour
    {
        [SerializeField] float boostMultiplier = 2f; // Boost factor
        [SerializeField] float boostDuration = 3f;  // How long the boost lasts
        [SerializeField] CarControls carControls;
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Wheels")
            {
                if (carControls != null)
                {
                    Debug.Log("Speed Boost Activated!");
                    carControls.ApplySpeedBoost(boostMultiplier, boostDuration);
                }
                //Rigidbody carRigidbody = other.GetComponent<Rigidbody>();
                //if (carRigidbody != null)
                //{
                //    // Apply a forward force to the car's Rigidbody
                //    carRigidbody.AddForce(other.transform.forward * boostForce, ForceMode.Impulse);

                //}
            }
        }
    }
}
