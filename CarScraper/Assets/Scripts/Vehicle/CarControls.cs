using UnityEngine;
using UnityEngine.InputSystem;

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
        private void Awake()
        {
            // Get components
            rigid = GetComponent<Rigidbody>();
        }

        void Update()
        {
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
    }
}