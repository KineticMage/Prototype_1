using UnityEngine;

namespace CarScraper
{
    public class SettingsMenu : MonoBehaviour
    {
        public bool isVisible = false;

        public void ShowSettings(GameObject settingsMenuUI)
        {
            Debug.Log("Settings Enabled");
            Time.timeScale = 0f;
            settingsMenuUI.SetActive(true);
            isVisible = true;
        }

        public void DisableSettings(GameObject settingsMenuUI)
        {
            Debug.Log("Settings Disabled");
            Time.timeScale = 1f;
            settingsMenuUI.SetActive(false);
            isVisible = false;
        }
    }
}
