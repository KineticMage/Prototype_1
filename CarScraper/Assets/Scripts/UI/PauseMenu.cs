using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace CarScraper
{
    public class PauseMenu : MonoBehaviour
    {
        public bool isPaused = false;

        public void PauseGame(GameObject pauseMenuUI)
        {
            Debug.Log("Paused");
            pauseMenuUI.SetActive(true);  
            Time.timeScale = 0f;         
            isPaused = true;
        }

        public void ResumeGame(GameObject pauseMenuUI)
        {
            Debug.Log("Resume");
            pauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
            isPaused = false;
        }

        public void RestartLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
