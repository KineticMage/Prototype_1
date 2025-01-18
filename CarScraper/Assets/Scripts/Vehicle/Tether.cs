using UnityEngine;
using UnityEngine.InputSystem;

namespace CarScraper
{
    public class Tether : MonoBehaviour
    {
        [SerializeField]
        private Vector3 tetherPoint;
        [SerializeField]
        private Rigidbody carRB;
        [SerializeField]
        private float maxDistance;
        private Vector3 dist;
        [SerializeField]
        private float torque;
        [SerializeField]
        private InputActionReference leftClick;
        private bool isTethered;
        private bool drawMouseHit;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            isTethered = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (isTethered)
            {
                dist = tetherPoint - carRB.transform.position;
                if (dist.magnitude >= maxDistance)
                {
                    carRB.transform.position = tetherPoint - dist.normalized * maxDistance;
                    carRB.AddForce(dist * 1000);
                    carRB.AddTorque(Vector3.up * torque);
                }
            }
            else
            {
                // Check for TetherableObjects when left click is pressed
                if (leftClick.action.ReadValue<float>() == 1)
                {
                    // Get the mouse's position
                    UnityEngine.Camera mainCam = UnityEngine.Camera.main;
                    Vector3 mousePos = Mouse.current.position.ReadValue();
                    drawMouseHit = true;

                    // Do a raycast, and if it hits a tetherable object, attach to it
                    LayerMask layerMask = LayerMask.GetMask("TetherObject");
                    Ray ray = mainCam.ScreenPointToRay(mousePos);
                    if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
                    {
                        isTethered = true;
                        tetherPoint = hit.transform.position;
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (isTethered)
                Gizmos.DrawRay(tetherPoint, -dist);
        }
    }
}
