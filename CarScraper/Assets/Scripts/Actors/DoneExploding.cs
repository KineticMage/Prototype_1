using UnityEngine;

namespace CarScraper
{
    public class DoneExploding : MonoBehaviour
    {


        public void Start()
        {
            Destroy(gameObject, GetComponent<ParticleSystem>().main.duration);

        }


    }
}
