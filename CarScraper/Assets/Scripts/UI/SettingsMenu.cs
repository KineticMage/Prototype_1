 using UnityEngine;
using UnityEngine.UI;

namespace CarScraper
{
    public class SettingsMenu : MonoBehaviour
    {
        public bool isVisible = false;
        [SerializeField] Slider brightnessSlider;
        [SerializeField] Image brightnessOverlay;

        public void EnableSettings(GameObject settingsMenuUI)
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

        public void ChangeBrightness()
        {
            Color tempColor = brightnessOverlay.color;
            tempColor.a = brightnessSlider.value;
            brightnessOverlay.color = tempColor;
        }
    }
}
