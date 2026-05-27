using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string gameplaySceneName = "Gameplay";

    [Header("Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject creditsPanel;

    [Header("Audio")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TMP_Text volumeValueText;

    [Header("Display")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    private readonly List<Resolution> uniqueResolutions = new List<Resolution>();

    private const string VolumeKey = "MASTER_VOLUME";
    private const string ResolutionKey = "RESOLUTION_INDEX";
    private const string FullscreenKey = "FULLSCREEN_STATE";

    private void Start()
    {
        ShowMainPanel();
        SetupVolume();
        SetupFullscreen();
        SetupResolutionDropdown();
    }

    // -------------------------
    // Buttons
    // -------------------------

    public void PlayGame()
    {
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void OpenOptions()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(true);
        if (creditsPanel != null) creditsPanel.SetActive(false);
    }

    public void OpenCredits()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(true);
    }

    public void BackToMain()
    {
        ShowMainPanel();
    }

    public void QuitGame()
    {
        PlayerPrefs.Save();

#if UNITY_EDITOR
        Debug.Log("QuitGame llamado. En el editor no se cerrará el juego.");
#else
        Application.Quit();
#endif
    }

    private void ShowMainPanel()
    {
        if (mainPanel != null) mainPanel.SetActive(true);
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);
    }

    // -------------------------
    // Volume
    // -------------------------

    private void SetupVolume()
    {
        // Cargar el volumen guardado, o 1f (100%) si no hay ninguno guardado.
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 1f);

        AudioListener.volume = savedVolume;

        if (volumeSlider != null)
        {
            volumeSlider.SetValueWithoutNotify(savedVolume);
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        UpdateVolumeText(savedVolume);
    }

    public void SetVolume(float value)
    {
        AudioListener.volume = value;

        UpdateVolumeText(value);

        PlayerPrefs.SetFloat(VolumeKey, value);
        PlayerPrefs.Save(); // Guardar inmediatamente después del cambio
    }

    private void UpdateVolumeText(float value)
    {
        if (volumeValueText != null)
        {
            int percent = Mathf.RoundToInt(value * 100f);
            volumeValueText.text = percent + "%";
        }
    }

    // -------------------------
    // Fullscreen
    // -------------------------

    private void SetupFullscreen()
    {
        // Cargar el estado de pantalla completa guardado, o el estado actual de la pantalla si no hay ninguno guardado.
        bool savedFullscreen = PlayerPrefs.GetInt(FullscreenKey, Screen.fullScreen ? 1 : 0) == 1;

        Screen.fullScreen = savedFullscreen;

        if (fullscreenToggle != null)
        {
            fullscreenToggle.SetIsOnWithoutNotify(savedFullscreen);
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        }
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;

        PlayerPrefs.SetInt(FullscreenKey, isFullscreen ? 1 : 0);
        PlayerPrefs.Save(); // Guardar inmediatamente después del cambio

        // Si hay resoluciones únicas, aplicar la resolución actual para que el cambio de pantalla completa surta efecto.
        if (uniqueResolutions.Count > 0)
        {
            int currentIndex = resolutionDropdown != null ? resolutionDropdown.value : 0;
            ApplyResolution(currentIndex);
        }
    }

    // -------------------------
    // Resolution
    // -------------------------

    private void SetupResolutionDropdown()
    {
        if (resolutionDropdown == null)
            return;

        resolutionDropdown.ClearOptions();
        uniqueResolutions.Clear();

        List<string> options = new List<string>();
        HashSet<string> addedResolutions = new HashSet<string>();

        int currentWidth = Screen.width;
        int currentHeight = Screen.height;
        int currentIndex = 0;

        Resolution[] allResolutions = Screen.resolutions;

        for (int i = 0; i < allResolutions.Length; i++)
        {
            Resolution res = allResolutions[i];

            string key = res.width + "x" + res.height;

            if (addedResolutions.Contains(key))
                continue;

            addedResolutions.Add(key);

            uniqueResolutions.Add(res);

            options.Add(res.width + " x " + res.height);

            if (res.width == currentWidth && res.height == currentHeight)
            {
                currentIndex = uniqueResolutions.Count - 1;
            }
        }

        resolutionDropdown.AddOptions(options);

        // Cargar el índice de resolución guardado, o el índice de la resolución actual si no hay ninguno guardado.
        int savedIndex = PlayerPrefs.GetInt(ResolutionKey, currentIndex);

        // Asegurarse de que el índice guardado esté dentro de los límites válidos.
        savedIndex = Mathf.Clamp(savedIndex, 0, uniqueResolutions.Count - 1);

        resolutionDropdown.SetValueWithoutNotify(savedIndex);
        resolutionDropdown.RefreshShownValue();

        resolutionDropdown.onValueChanged.AddListener(SetResolution);

        ApplyResolution(savedIndex);
    }

    public void SetResolution(int index)
    {
        ApplyResolution(index);
    }

    private void ApplyResolution(int index)
    {
        if (uniqueResolutions.Count == 0)
            return;

        index = Mathf.Clamp(index, 0, uniqueResolutions.Count - 1);

        Resolution selectedResolution = uniqueResolutions[index];

        Screen.SetResolution(
            selectedResolution.width,
            selectedResolution.height,
            Screen.fullScreen
        );

        PlayerPrefs.SetInt(ResolutionKey, index);
        PlayerPrefs.Save(); // Guardar inmediatamente después del cambio
    }
}