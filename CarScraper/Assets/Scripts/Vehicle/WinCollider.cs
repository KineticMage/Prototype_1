using UnityEngine;

namespace CarScraper
{
    public class WinCollider : MonoBehaviour
    {
        [SerializeField] GameObject GameWonUi;
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Wheels")
            {
                GameWonUi.SetActive(true);
            }
        }
    }
}
