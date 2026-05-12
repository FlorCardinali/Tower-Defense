using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro; // Simplifica el uso de TMP_Dropdown para resolucion de pantalla por ejemplo

public class MainMenu : MonoBehaviour
{
    [Header("--- PANELES DE MENÚ ---")]
    [Tooltip("Panel principal con Jugar, Ajustes y Salir")]
    public GameObject main_Menu_Panel;
    [Tooltip("Panel intermedio de Ajustes")]
    public GameObject settings_Panel;
    [Tooltip("Subpanel de configuración de Audio")]
    public GameObject audio_Settings_Panel;
    [Tooltip("Subpanel de configuración de Video")]
    public GameObject video_Panel;

    [Header("--- CONFIGURACIÓN DE AUDIO ---")]
    public AudioMixer masterMixer;

    [Header("--- CONFIGURACIÓN DE VIDEO ---")]
    public TMP_Dropdown resolutionDropdown;

    // Variables temporales para el sistema de "Aplicar"
    private int tempResIndex;
    private int tempShadowIndex;
    private int tempTextureIndex;
    private int tempAAIndex;

    #region Metodos de Unity

    void Start()
    {
        // Aseguramos el estado inicial de la UI
        ResetPanels();

        // Inicializamos la lista de resoluciones
        ConfigurarResoluciones();
    }

    #endregion

    #region Navegación entre Menús

    private void ResetPanels()
    {
        main_Menu_Panel.SetActive(true);
        settings_Panel.SetActive(false);
        audio_Settings_Panel.SetActive(false);
        video_Panel.SetActive(false);
    }

    public void AbrirAjustes()
    {
        main_Menu_Panel.SetActive(false);
        settings_Panel.SetActive(true);
    }

    public void AbrirAudio()
    {
        settings_Panel.SetActive(false);
        audio_Settings_Panel.SetActive(true);
    }

    public void AbrirVideo()
    {
        settings_Panel.SetActive(false);
        video_Panel.SetActive(true);
    }

    public void VolverAlMenuPrincipal()
    {
        settings_Panel.SetActive(false);
        main_Menu_Panel.SetActive(true);
    }

    public void VolverAAjustesDesdeAudio()
    {
        audio_Settings_Panel.SetActive(false);
        settings_Panel.SetActive(true);
    }

    public void VolverAAjustesDesdeVideo()
    {
        video_Panel.SetActive(false);
        settings_Panel.SetActive(true);
    }

    #endregion

    #region Lógica de Juego

    public void Jugar()
    {
        // Carga la escena siguiente en el Build Settings
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Salir()
{
    Debug.Log("Saliendo del juego...");

    // Si estamos en el editor de Unity
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        // Si es el juego está exportado
        Application.Quit();
    #endif
}

    #endregion

    #region Control de Audio

    public void SetMasterVolume(float volume)
    {
        masterMixer.SetFloat("Master_Volume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
    }

    public void SetMusicVolume(float volume)
    {
        masterMixer.SetFloat("Music_Volume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
    }

    public void SetSFXVolume(float volume)
    {
        masterMixer.SetFloat("SFX_Volume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
    }

    #endregion

    #region Control de Video y Gráficos

    private void ConfigurarResoluciones()
    {
        Resolution[] resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();

        int currentResIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResIndex;
        resolutionDropdown.RefreshShownValue();

        tempResIndex = currentResIndex; // Inicializamos temp
    }

    // Métodos para capturar valores de los Dropdowns
    public void SetResolutionTemp(int index) => tempResIndex = index;
    public void SetShadowsTemp(int index) => tempShadowIndex = index;
    public void SetTextureTemp(int index) => tempTextureIndex = index;
    public void SetAATemp(int index) => tempAAIndex = index;

    public void AplicarCambiosVideo()
    {
        // 1. Resolución
        Resolution[] resolutions = Screen.resolutions;
        Resolution res = resolutions[tempResIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);

        // 2. Sombras
        if (tempShadowIndex == 0) QualitySettings.shadows = ShadowQuality.Disable;
        else if (tempShadowIndex == 1) QualitySettings.shadows = ShadowQuality.HardOnly;
        else QualitySettings.shadows = ShadowQuality.All;

        // 3. Texturas
        QualitySettings.globalTextureMipmapLimit = tempTextureIndex;

        // 4. Antialiasing
        int valorAA = (tempAAIndex == 1) ? 2 : (tempAAIndex == 2) ? 4 : (tempAAIndex == 3) ? 8 : 0;
        QualitySettings.antiAliasing = valorAA;

        Debug.Log("Configuración de video aplicada exitosamente.");
    }

    #endregion
}