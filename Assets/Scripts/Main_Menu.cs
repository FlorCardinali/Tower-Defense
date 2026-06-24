using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("--- PANELES DE MENÚ ---")]
    public GameObject main_Menu_Panel;
    public GameObject settings_Panel;
    public GameObject audio_Settings_Panel;
    public GameObject video_Panel;
    [Tooltip("Panel de créditos que creamos hoy")]
    public GameObject credito_Panel; 

    [Header("--- CONFIGURACIÓN DE AUDIO ---")]
    public AudioMixer masterMixer;

    [Header("--- CONFIGURACIÓN DE VIDEO ---")]
    public TMP_Dropdown resolutionDropdown;

    private int tempResIndex;
    private int tempShadowIndex;
    private int tempTextureIndex;
    private int tempAAIndex;

    void Start()
    {
        ResetPanels();
        ConfigurarResoluciones();
    }

    #region Navegación entre Menús

    private void ResetPanels()
    {
        main_Menu_Panel.SetActive(true);
        settings_Panel.SetActive(false);
        audio_Settings_Panel.SetActive(false);
        video_Panel.SetActive(false);
        credito_Panel.SetActive(false); // ¡Importante!
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

    // NUEVOS MÉTODOS DE CRÉDITOS
    public void AbrirCreditos()
    {
        main_Menu_Panel.SetActive(false);
        credito_Panel.SetActive(true);
    }

    public void VolverDesdeCreditos()
    {
        credito_Panel.SetActive(false);
        main_Menu_Panel.SetActive(true);
    }
    // FIN NUEVOS MÉTODOS

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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Salir()
    {
        Debug.Log("Saliendo del juego...");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    #endregion

    #region Control de Audio
    // ... (Tu código de audio se mantiene igual)
    public void SetMasterVolume(float volume) => masterMixer.SetFloat("Master_Volume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
    public void SetMusicVolume(float volume) => masterMixer.SetFloat("Music_Volume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
    public void SetSFXVolume(float volume) => masterMixer.SetFloat("SFX_Volume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
    #endregion

    #region Control de Video
    // ... (Tu código de video se mantiene igual)
    private void ConfigurarResoluciones() { /* Tu código existente */ }
    public void SetResolutionTemp(int index) => tempResIndex = index;
    public void SetShadowsTemp(int index) => tempShadowIndex = index;
    public void SetTextureTemp(int index) => tempTextureIndex = index;
    public void SetAATemp(int index) => tempAAIndex = index;

    public void AplicarCambiosVideo() { /* Tu código existente */ }
    #endregion
}