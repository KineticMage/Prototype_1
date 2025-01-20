using UnityEngine;
using UnityEngine.InputSystem;

namespace CarScraper.Vehicles
{
    public class CarControls : MonoBehaviour
    {
        public Rigidbody rigid;
        public WheelCollider wheel_L_F, wheel_L_B, wheel_R_F, wheel_R_B;
        public float drivespeed, steerspeed;

        [Header("InputMappings")]
        private Vector2 _moveDirection;
        [SerializeField] InputActionReference move;
        [SerializeField] InputActionReference jumpInput;

        [Header("Jumping")]
        public float jumpForce;
        public float FallMultiplier = 2.5f;
        public float lowJumpMultiplier = 2f;
        private bool isJumping = false;

        public Rigidbody RB => rigid;

        private void Awake()
        {
            // Get Rigidbody component
            rigid = GetComponent<Rigidbody>();

            // **Option 1: Lower the Center of Mass**
            rigid.centerOfMass = new Vector3(0, -0.5f, 0); // Adjust Y value as needed
        }

        void Update()
        {
            if (rigid.linearVelocity.z < 0 && isJumping)
            {
                rigid.linearVelocity += Vector3.up * Physics.gravity.y * (FallMultiplier - 1) * Time.deltaTime;
            }
            else if (rigid.linearVelocity.z > 0 && !jumpInput.action.IsPressed() && isJumping)
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

            // **Option 2: Apply Anti-Roll Bars**
            ApplyAntiRollBar(wheel_L_F, wheel_R_F);
            ApplyAntiRollBar(wheel_L_B, wheel_R_B);
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

        // **Option 2: Anti-Roll Bar Implementation**
        void ApplyAntiRollBar(WheelCollider leftWheel, WheelCollider rightWheel)
        {
            WheelHit hit;
            float leftTravel = leftWheel.GetGroundHit(out hit) ? 1 - (hit.point.y - leftWheel.transform.position.y) / leftWheel.suspensionDistance : 1;
            float rightTravel = rightWheel.GetGroundHit(out hit) ? 1 - (hit.point.y - rightWheel.transform.position.y) / rightWheel.suspensionDistance : 1;

            float antiRollForce = (leftTravel - rightTravel) * 5000f; // Adjust force

            if (leftWheel.isGrounded) rigid.AddForceAtPosition(leftWheel.transform.up * -antiRollForce, leftWheel.transform.position);
            if (rightWheel.isGrounded) rigid.AddForceAtPosition(rightWheel.transform.up * antiRollForce, rightWheel.transform.position);
        }
    }
}
