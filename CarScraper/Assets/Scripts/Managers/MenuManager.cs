using UnityEngine;
using UnityEngine.InputSystem;

namespace CarScraper
{
    public class MenuManager : MonoBehaviour
    {
        [Header("Pause")]
        [SerializeField] InputActionReference pause;
        [SerializeField] PauseMenu pauseMenu;
        [SerializeField] GameObject pauseMenuUI;

        [Header("Settings")]
        [SerializeField] InputActionReference settings;
        [SerializeField] SettingsMenu settingsMenu;
        [SerializeField] GameObject settingsMenuUI;

        private void OnEnable()
        {
            pause.action.started += PauseLogic; 
            settings.action.started += SettingsLogic; 
        }

        private void OnDisable()
        {
            pause.action.started -= PauseLogic; 
            settings.action.started -= SettingsLogic; 
        }
        public void PauseLogic(InputAction.CallbackContext ctx)
        {
            if (pauseMenu.isPaused) pauseMenu.ResumeGame(pauseMenuUI);
            else pauseMenu.PauseGame(pauseMenuUI);
        }
        public void SettingsLogic(InputAction.CallbackContext ctx)
        {
            if (settingsMenu.isVisible) settingsMenu.DisableSettings(settingsMenuUI);
            else settingsMenu.ShowSettings(settingsMenuUI);
        }
    }
}
