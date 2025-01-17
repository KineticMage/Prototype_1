using UnityEngine;

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

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            dist = tetherPoint - carRB.transform.position;
            if (dist.magnitude >= maxDistance)
            {
                carRB.transform.position = tetherPoint - dist.normalized * maxDistance;
                carRB.AddForce(dist * 1000);
                carRB.AddTorque(Vector3.up * torque);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawRay(tetherPoint, -dist);
        }
    }
}
