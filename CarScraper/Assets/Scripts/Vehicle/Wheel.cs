using UnityEngine;

namespace CarScraper.Vehicles
{
    public class Wheel : MonoBehaviour
    {
        public WheelCollider wheelCollider;
        public Transform wheelMesh;
        public bool wheelTurn;

        void Update()
        {
            if (wheelTurn == true)
            {
                wheelMesh.localEulerAngles = new Vector3(wheelMesh.localEulerAngles.x, wheelCollider.steerAngle - wheelMesh.localEulerAngles.z, wheelMesh.localEulerAngles.z);
            }
            wheelMesh.Rotate(0, wheelCollider.rpm / 60 * 360 * Time.deltaTime, 0);
        }
    }
}
