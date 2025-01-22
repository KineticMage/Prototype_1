using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

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
        private List<GameObject> tetherObjsInRange;
        private GameObject highlightedTetherObj;

        void Start()
        {
            isTethered = false;
            tetherObjsInRange = new List<GameObject>();
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
                // Remove hits that are too far away from the list
                for (int i = 0; i < tetherObjsInRange.Count; i++)
                {
                    GameObject go = tetherObjsInRange[i];
                    float currentDist = (go.transform.position - carRB.transform.position).magnitude;
                    if (currentDist > maxDistance)
                    {
                        if (go == highlightedTetherObj)
                            highlightedTetherObj = null;
                        tetherObjsInRange.RemoveAt(i);
                        i--;
                    }
                }

                // Add new hits to the list
                LayerMask layerMask = LayerMask.GetMask("TetherObject");
                RaycastHit[] hits = Physics.SphereCastAll(carRB.transform.position, maxDistance, Vector3.up,
                    maxDistance, layerMask);
                foreach (RaycastHit hit in hits)
                {
                    GameObject go = hit.collider.gameObject;
                    if (!tetherObjsInRange.Contains(go))
                    {
                        tetherObjsInRange.Add(go);
                    }
                }

                // If there is no closest hit, highlight the closest hit
                if (highlightedTetherObj == null)
                {
                    float minDistance = maxDistance;
                    foreach (GameObject go in tetherObjsInRange)
                    {
                        float currentDist = (go.transform.position - carRB.transform.position).magnitude;
                        if (currentDist < minDistance)
                        {
                            minDistance = currentDist;
                            highlightedTetherObj = go;
                        }
                    }

                    if (highlightedTetherObj != null)
                    {
                        // Set material here
                    }
                }

                if (leftClick.action.ReadValue<float>() == 1 && highlightedTetherObj != null)
                {
                    isTethered = true;
                    tetherPoint = highlightedTetherObj.transform.position;
                    tetherCube.SetActive(true);

                    dist = tetherPoint - carRB.transform.position; // Update dist before using it
                    tetherCube.transform.localScale = new Vector3(0.1f, 0.1f, dist.magnitude);
                    tetherCube.transform.localPosition = tetherPoint - dist / 2;
                    tetherCube.transform.rotation = Quaternion.LookRotation(dist);
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
