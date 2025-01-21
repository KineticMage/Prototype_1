using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

namespace CarScraper.Vehicles
{
    public class CarControls : MonoBehaviour
    {
        public Rigidbody rigid;
        public WheelCollider wheel_L_F, wheel_L_B, wheel_R_F, wheel_R_B;
        public float drivespeed, steerspeed;
        //float horizontalInput, verticalInput;

        [Header("InputMappings")]
        private Vector2 _moveDirection;
        [SerializeField] InputActionReference move;
        [SerializeField] InputActionReference jumpInput;

        [Header("Jumping")]
        public float jumpForce;
        public float FallMultiplier = 2.5f;
        public float lowJumpMultiplier = 2f;
        private bool isJumping = false;

        [Header("Speed Boost")]
        //float motor; // Base motor torque 
        public float boostMultiplier = 2f; // How much to multiply torque during boost
        private float currentMotorTorque;
        private bool isBoosting = false;

        public Rigidbody RB => rigid;

        private void Awake()
        {
            // Get components
            rigid = GetComponent<Rigidbody>();
        }

        void Update()
        {
            

            if(rigid.linearVelocity.z<0 && isJumping)
            {
                rigid.linearVelocity += Vector3.up * Physics.gravity.y * (FallMultiplier - 1) * Time.deltaTime;
            }
            else if (rigid.linearVelocity.z>0  &&  !jumpInput.action.IsPressed()&& isJumping)
            {
                rigid.linearVelocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
            }
            _moveDirection = move.action.ReadValue<Vector2>();
        }

        void FixedUpdate()
        {
            float motor = _moveDirection.y * drivespeed;
            wheel_L_F.motorTorque = motor;
            wheel_L_B.motorTorque = motor;
            wheel_R_F.motorTorque = motor;
            wheel_R_B.motorTorque = motor;
          
            wheel_L_F.steerAngle = steerspeed * _moveDirection.x;
            wheel_L_B.steerAngle = steerspeed * _moveDirection.x;
            wheel_R_F.steerAngle = steerspeed * _moveDirection.x;
            wheel_L_F.steerAngle = steerspeed * _moveDirection.x;
        }

        public void Jump()
        {
            Debug.Log("jump!");
            if (!isJumping)
            {
                rigid.linearVelocity += Vector3.up * jumpForce;
                isJumping = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Ground")
            {
                isJumping = false;
            }
        }


        public void ApplySpeedBoost(float boostMultiplier, float boostDuration)
        {
            if (!isBoosting)
            {
                StartCoroutine(SpeedBoostRoutine(boostMultiplier, boostDuration));
            }
        }

        private IEnumerator SpeedBoostRoutine(float boostMultiplier, float duration)
        {
            isBoosting = true;

            // Temporarily increase motor torque
            wheel_L_F.motorTorque = drivespeed * boostMultiplier;
            wheel_L_B.motorTorque = drivespeed * boostMultiplier;
            wheel_R_F.motorTorque = drivespeed * boostMultiplier;
            wheel_R_B.motorTorque = drivespeed * boostMultiplier;
            Debug.Log($"Speed boosted to: {wheel_R_F.motorTorque}");

            yield return new WaitForSeconds(duration);

            // Revert motor torque to normal
            wheel_L_F.motorTorque = wheel_L_F.motorTorque;
            wheel_L_B.motorTorque = wheel_L_B.motorTorque;
            wheel_R_F.motorTorque = wheel_R_F.motorTorque;
            wheel_R_B.motorTorque = wheel_R_B.motorTorque;
            Debug.Log("Speed boost ended.");
            isBoosting = false;
        }
    }
}