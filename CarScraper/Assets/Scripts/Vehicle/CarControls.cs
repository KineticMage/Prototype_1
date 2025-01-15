using UnityEngine;

namespace CarScraper.Vehicles
{
    public class CarControls : MonoBehaviour
    {
        public Rigidbody rigid;
        public WheelCollider wheel_L_F, wheel_L_B, wheel_R_F, wheel_R_B;
        public float drivespeed, steerspeed;
        float horizontalInput, verticalInput;

        private void Awake()
        {
            // Get components
            rigid = GetComponent<Rigidbody>();
        }

        void Update()
        {
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");
        }

        void FixedUpdate()
        {
            float motor = Input.GetAxis("Vertical") * drivespeed;
            wheel_L_F.motorTorque = motor;
            wheel_L_B.motorTorque = motor;
            wheel_R_F.motorTorque = motor;
            wheel_R_B.motorTorque = motor;
            wheel_L_F.steerAngle = steerspeed * horizontalInput;
            wheel_L_B.steerAngle = steerspeed * horizontalInput;
        }
    }
}