using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace CarScraper
{
    enum TetherType
    {
        Attach,
        Drag
    }

    public class Tether : MonoBehaviour
    {
        [SerializeField]
        private GameObject tetherPoint;
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
        [SerializeField]
        private InputActionReference scroll;
        private bool isTethered;
        [SerializeField]
        private GameObject tetherCube;
        private List<GameObject> tetherObjsInRange;
        private GameObject highlightedTetherObj;
        private TetherType tetherType;

        void Start()
        {
            isTethered = false;
            tetherObjsInRange = new List<GameObject>();
        }

        void Update()
        {
            if (isTethered)
            {
                dist = tetherPoint.transform.position - carRB.transform.position;

                if (dist.magnitude >= maxDistance)
                {
                    if (tetherType == TetherType.Attach)
                    {
                        carRB.transform.position = tetherPoint.transform.position - dist.normalized * maxDistance;
                        carRB.AddForce(dist * 1000);
                        carRB.AddTorque(Vector3.up * torque);
                    }
                    else
                    {
                        tetherPoint.transform.position = carRB.transform.position + dist.normalized * maxDistance;
                        if (tetherPoint.TryGetComponent<Rigidbody>(out Rigidbody rb))
                            rb.AddForce(-dist * rb.mass);
                    }
                }

                tetherCube.transform.localScale = new Vector3(0.1f, 0.1f, dist.magnitude);
                tetherCube.transform.localPosition = tetherPoint.transform.position - dist / 2;
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
                        go.GetComponent<MeshRenderer>().material.color = Color.white;
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
                        go.GetComponent<MeshRenderer>().material.color = Color.yellow;
                    }
                }

                // If there is no closest hit, highlight the closest hit
                if (highlightedTetherObj == null && tetherObjsInRange.Count > 0)
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
                        highlightedTetherObj.GetComponent<MeshRenderer>().material.color = Color.green;
                    }
                }

                float tetherFocusVal = scroll.action.ReadValue<Vector2>().y;
                if (tetherFocusVal != 0f && tetherObjsInRange.Count > 1)
                {
                    highlightedTetherObj.GetComponent<MeshRenderer>().material.color = Color.yellow;
                    int focusedTetherObjIdx = tetherObjsInRange.IndexOf(highlightedTetherObj);
                    if (tetherFocusVal < 0f)
                        focusedTetherObjIdx--;
                    else
                        focusedTetherObjIdx++;
                    focusedTetherObjIdx %= tetherObjsInRange.Count;
                    if (focusedTetherObjIdx < 0)
                        focusedTetherObjIdx = tetherObjsInRange.Count - 1;
                    highlightedTetherObj = tetherObjsInRange[focusedTetherObjIdx];
                    highlightedTetherObj.GetComponent<MeshRenderer>().material.color = Color.green;
                }

                if (leftClick.action.ReadValue<float>() == 1 && highlightedTetherObj != null)
                {
                    isTethered = true;
                    tetherPoint = highlightedTetherObj;
                    tetherCube.SetActive(true);
                    if (tetherPoint.tag == "Draggable")
                        tetherType = TetherType.Drag;
                    else
                        tetherType = TetherType.Attach;

                    dist = tetherPoint.transform.position - carRB.transform.position; // Update dist before using it
                    tetherCube.transform.localScale = new Vector3(0.1f, 0.1f, dist.magnitude);
                    tetherCube.transform.localPosition = tetherPoint.transform.position - dist / 2;
                    tetherCube.transform.rotation = Quaternion.LookRotation(dist);
                }
            }
        }
    }
}
