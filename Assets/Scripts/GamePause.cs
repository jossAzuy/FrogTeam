using UnityEngine;
using UnityEngine.InputSystem;

public class GamePause : MonoBehaviour
{
    public GameObject pauseMenu;
    public bool isGamePaused = false;

    private void Update()
    {
        
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isGamePaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        isGamePaused = false;
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        isGamePaused = true;
    }
}