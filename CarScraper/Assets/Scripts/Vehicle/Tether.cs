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
                    // Get the mouse's world position
                    UnityEngine.Camera mainCam = UnityEngine.Camera.main;
                    Vector3 mousePos = Mouse.current.position.ReadValue();
                    mousePos.z = mainCam.transform.position.y;
                    Vector3 screenToWorld = mainCam.ScreenToWorldPoint(mousePos);
                    Debug.Log(screenToWorld);
                    drawMouseHit = true;

                    // Do a raycast, and if it hits a tetherable object, attach to it
                    LayerMask layerMask = LayerMask.GetMask("TetherObject");
                    if (Physics.SphereCast(screenToWorld, 1, Vector3.up, out RaycastHit hit, Mathf.Infinity, layerMask))
                    {
                        isTethered = true;
                        tetherPoint = hit.transform.position;
                        Debug.Log("hruhrugh");
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (isTethered)
                Gizmos.DrawRay(tetherPoint, -dist);
            if (drawMouseHit)
            {
                // Get the mouse's world position
                UnityEngine.Camera mainCam = UnityEngine.Camera.main;
                Vector3 mousePos = Mouse.current.position.ReadValue();
                mousePos.z = mainCam.transform.position.y;
                Vector3 screenToWorld = UnityEngine.Camera.main.ScreenToWorldPoint(mousePos);
                Gizmos.DrawSphere(screenToWorld, 1);
                drawMouseHit = false;
            }
        }
    }
}
