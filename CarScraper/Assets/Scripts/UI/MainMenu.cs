using UnityEngine;
using UnityEngine.SceneManagement;

namespace CarScraper
{
    public class MainMenu : MonoBehaviour
    {
        public void PlayGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        public void Quit()
        {
            Debug.Log("Quit");
            Application.Quit();
        }
    }
}
