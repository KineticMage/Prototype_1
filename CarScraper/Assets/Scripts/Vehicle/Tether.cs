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
        [SerializeField]
        private InputActionReference rightClick;
        private bool isTethered;
        [SerializeField]
        private GameObject tetherCube;
        private GameObject[] tetherObjsInRange;

        void Start()
        {
            isTethered = false;
        }

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

                tetherCube.transform.localScale = new Vector3(0.1f, 0.1f, dist.magnitude);
                tetherCube.transform.localPosition = tetherPoint - dist / 2;
                tetherCube.transform.rotation = Quaternion.LookRotation(dist);

                if (leftClick.action.ReadValue<float>() == 0)
                {
                    isTethered = false;
                    tetherCube.SetActive(false);
                }
            }
            else
            {
                if (leftClick.action.ReadValue<float>() == 1)
                {
                    UnityEngine.Camera mainCam = UnityEngine.Camera.main; Vector3 mousePos = Mouse.current.position.ReadValue();
                    LayerMask layerMask = LayerMask.GetMask("TetherObject");
                    Ray ray = mainCam.ScreenPointToRay(mousePos);

                    if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
                    {
                        float distanceToHit = Vector3.Distance(hit.point, carRB.transform.position);

                        // Ensure the hit point is within the max tethering range
                        if (distanceToHit <= maxDistance)
                        {
                            isTethered = true;
                            tetherPoint = hit.point;
                            tetherCube.SetActive(true);

                            dist = tetherPoint - carRB.transform.position; // Update dist before using it
                            tetherCube.transform.localScale = new Vector3(0.1f, 0.1f, dist.magnitude);
                            tetherCube.transform.localPosition = tetherPoint - dist / 2;
                            tetherCube.transform.rotation = Quaternion.LookRotation(dist);
                        }
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (isTethered)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(tetherPoint, 0.2f);
                Gizmos.DrawLine(carRB.transform.position, tetherPoint);
            }
        }
    }
}
