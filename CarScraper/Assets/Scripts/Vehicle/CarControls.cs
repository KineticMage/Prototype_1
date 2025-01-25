using UnityEngine;
using UnityEngine.InputSystem;

namespace CarScraper.Vehicles
{
    public class CarControls : MonoBehaviour
    {
        public Rigidbody rigid;
        public WheelCollider wheel_L_F, wheel_L_B, wheel_R_F, wheel_R_B;
        public float drivespeed, steerspeed;

        [Header("Input Mappings")]
        private Vector2 _moveDirection;
        [SerializeField] InputActionReference move;
        [SerializeField] InputActionReference jumpInput;

        [Header("Jumping")]
        public float jumpForce;
        public float fallMultiplier = 2.5f;
        public float lowJumpMultiplier = 2f;
        private bool isGrounded = true;

        [Header("Car Recovery")]
        public float flipCooldown = 3f; // Time before auto-flipping
        private float lastFlipTime = 0f;

        private void Awake()
        {
            rigid = GetComponent<Rigidbody>();
            rigid.centerOfMass = new Vector3(0, -0.5f, 0);
        }

        void Update()
        {
            _moveDirection = move.action.ReadValue<Vector2>();

            if (jumpInput.action.WasPressedThisFrame() && isGrounded)
            {
                Jump();
            }

            ApplyGravityMultipliers();

            // Check if the car is flipped and needs to reset
            if (IsCarFlipped() && Time.time > lastFlipTime + flipCooldown)
            {
                ResetCarPosition();
                lastFlipTime = Time.time;
            }

            // Manual reset with "R" key
            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                ResetCarPosition();
            }
        }

        void FixedUpdate()
        {
            float motor = _moveDirection.y * drivespeed * -1;
            wheel_L_F.motorTorque = motor;
            wheel_L_B.motorTorque = motor;
            wheel_R_F.motorTorque = motor;
            wheel_R_B.motorTorque = motor;

            float steer = steerspeed * _moveDirection.x;
            wheel_L_F.steerAngle = steer;
            wheel_R_F.steerAngle = steer;

            ApplyAntiRollBar(wheel_L_F, wheel_R_F);
            ApplyAntiRollBar(wheel_L_B, wheel_R_B);

            ApplyStabilization();
        }

        private void Jump()
        {
            Debug.Log("Jump!");
            rigid.linearVelocity = new Vector3(rigid.linearVelocity.x, 0, rigid.linearVelocity.z); // Reset Y velocity
            rigid.AddForce(Vector3.up * jumpForce * rigid.mass, ForceMode.Impulse);
            isGrounded = false;
        }

        private void ApplyGravityMultipliers()
        {
            if (rigid.linearVelocity.y < 0)
            {
                rigid.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }
            else if (rigid.linearVelocity.y > 0 && !jumpInput.action.IsPressed())
            {
                rigid.linearVelocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.CompareTag("Ground"))
            {
                isGrounded = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Ground"))
            {
                isGrounded = false;
            }
        }

        private bool IsCarFlipped()
        {
            return Vector3.Dot(transform.up, Vector3.up) < 0.2f; // Less than 20% upright means it's flipped
        }

        private void ResetCarPosition()
        {
            Debug.Log("Car Reset!");

            // Smoothly reset rotation to upright position
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(transform.forward, Vector3.up), 0.5f);

            // Reset velocity to prevent weird physics glitches
            rigid.linearVelocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;

            // Lift the car slightly to avoid ground clipping
            transform.position += Vector3.up * 1.5f;
        }

        void ApplyStabilization()
        {
            Vector3 currentUp = transform.up;
            Vector3 desiredUp = Vector3.up;
            Vector3 stabilizationTorque = Vector3.Cross(currentUp, desiredUp) * 500f; // Adjust force if needed
            rigid.AddTorque(stabilizationTorque, ForceMode.Force);
        }

        void ApplyAntiRollBar(WheelCollider leftWheel, WheelCollider rightWheel)
        {
            WheelHit hit;
            float leftTravel = leftWheel.GetGroundHit(out hit) ? 1 - (hit.point.y - leftWheel.transform.position.y) / leftWheel.suspensionDistance : 1;
            float rightTravel = rightWheel.GetGroundHit(out hit) ? 1 - (hit.point.y - rightWheel.transform.position.y) / rightWheel.suspensionDistance : 1;

            if (!leftWheel.isGrounded || !rightWheel.isGrounded) return;

            float antiRollForce = (leftTravel - rightTravel) * 5000f;

            rigid.AddForceAtPosition(leftWheel.transform.up * -antiRollForce, leftWheel.transform.position);
            rigid.AddForceAtPosition(rightWheel.transform.up * antiRollForce, rightWheel.transform.position);
        }
    }
}
