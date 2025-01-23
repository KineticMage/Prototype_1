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
        }

        void FixedUpdate()
        {
            float motor = _moveDirection.y * drivespeed *-1;
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
            rigid.AddForce(Vector3.up * jumpForce*rigid.mass,ForceMode.Impulse);
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

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                isGrounded = true;
            }
        }
        void ApplyStabilization()
        {
            Vector3 currentUp = transform.up;
            Vector3 desiredUp = Vector3.up;

            // Compute a torque to correct the car's tilt
            Vector3 stabilizationTorque = Vector3.Cross(currentUp, desiredUp) * 500f; // Adjust force if needed
            rigid.AddTorque(stabilizationTorque, ForceMode.Force);
        }

        void ApplyAntiRollBar(WheelCollider leftWheel, WheelCollider rightWheel)
        {
            WheelHit hit;
            float leftTravel = leftWheel.GetGroundHit(out hit) ? 1 - (hit.point.y - leftWheel.transform.position.y) / leftWheel.suspensionDistance : 1;
            float rightTravel = rightWheel.GetGroundHit(out hit) ? 1 - (hit.point.y - rightWheel.transform.position.y) / rightWheel.suspensionDistance : 1;

            float antiRollForce = (leftTravel - rightTravel) * 5000f;

            if (leftWheel.isGrounded) rigid.AddForceAtPosition(leftWheel.transform.up * -antiRollForce, leftWheel.transform.position);
            if (rightWheel.isGrounded) rigid.AddForceAtPosition(rightWheel.transform.up * antiRollForce, rightWheel.transform.position);
        }
    }
}
