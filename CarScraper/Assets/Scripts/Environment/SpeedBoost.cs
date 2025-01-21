using System.Collections;
using CarScraper.Vehicles;
using UnityEngine;
using UnityEngine.InputSystem.XR;

namespace CarScraper
{
    public class SpeedBoost : MonoBehaviour
    {
        [Header("SpeedBoost")]
        [SerializeField] float boostForce = 1000f; // Force to apply
        [SerializeField] float boostDuration = 3f;  // How long the boost lasts

        [Header("LinearDamping")]
        [SerializeField] float maxlinearDamping = 5f;
        [SerializeField] float dampingDuration = 1f;
        private float originallinearDamping = 0f;

        private Rigidbody carRb;

        private void Awake()
        {
            carRb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
            originallinearDamping = carRb.linearDamping;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Wheels")
            {
                if (carRb != null)
                {
                    StartCoroutine(ApplySpeedBoost(carRb, boostDuration));
                }
            }
        }

        private IEnumerator ApplySpeedBoost(Rigidbody rb, float boostDuration)
        {
            float elapsed = 0f;

            while (elapsed < boostDuration)
            {
                Debug.Log("Speed Boost Activated!");
                rb.AddForce(transform.forward * boostForce * Time.deltaTime, ForceMode.Acceleration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            Debug.Log("Stopped");

            //Adding linear damping to ease down the vehicle stopping
            yield return StartCoroutine(GraduallyReducelinearDamping(rb, maxlinearDamping, originallinearDamping, dampingDuration));
        }

        private IEnumerator GraduallyReducelinearDamping(Rigidbody rb, float maxlinearDamping, float originallinearDamping, float dampingDuration)
        {
            rb.linearDamping = maxlinearDamping;
            float elapsed = 0f;

            while (elapsed < dampingDuration)
            {
                //rb.linearDamping = Mathf.Lerp(maxlinearDamping, originallinearDamping, 1f - Mathf.Exp(-elapsed / dampingDuration));
                rb.linearDamping = Mathf.Lerp(maxlinearDamping, originallinearDamping, elapsed / dampingDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            rb.linearDamping = originallinearDamping; 
        }
    }
}
