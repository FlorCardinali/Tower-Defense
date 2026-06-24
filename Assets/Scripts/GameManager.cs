using UnityEngine;
using System;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Referencias de Jugadores")]
    [SerializeField] private Health_controller player1;
    [SerializeField] private Health_controller player2;

    [Header("Configuración del Video de Victoria")]
    [SerializeField] private GameObject objetoPantallaVideo;
    [SerializeField] private VideoPlayer videoPlayerVictoria;
    [SerializeField] private string nombreSiguienteEscena = "EscenaCreditos";

    [Header("Configuración del Video de Derrota")]
    [SerializeField] private GameObject objetoPantallaVideoDerrota;
    [SerializeField] private VideoPlayer videoPlayerDerrota;
    [SerializeField] private string nombreEscenaGameOver = "PantallaPerdiste";

    private AudioSource musicaFondo;

    public event Action OnBossDerrotado;
    private bool secuenciaFinalIniciada = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        musicaFondo = GetComponent<AudioSource>();
    }

    void Start()
    {
        // === ASEGURAR TIEMPO NORMAL AL INICIAR ===
        Time.timeScale = 1f;

        if (videoPlayerVictoria != null)
            videoPlayerVictoria.loopPointReached += AlTerminarVideoVictoria;

        if (videoPlayerDerrota != null)
            videoPlayerDerrota.loopPointReached += AlTerminarVideoDerrota;
    }

    void Update()
    {
        if (secuenciaFinalIniciada) return;

        if (player1 == null && player2 == null)
        {
            secuenciaFinalIniciada = true;
            ActivarVideoDerrota();
        }
    }

    public void NotificarMuerteBoss()
    {
        OnBossDerrotado?.Invoke();
    }

    public void ActivarVideoVictoria()
    {
        if (secuenciaFinalIniciada) return;
        secuenciaFinalIniciada = true;

        if (musicaFondo != null) musicaFondo.Stop();

        // === PAUSAR EL JUEGO ===
        Time.timeScale = 0f;

        Debug.Log("GameManager: ¡Señal de victoria! Juego pausado y reproduciendo video...");
        if (objetoPantallaVideo != null && videoPlayerVictoria != null)
        {
            objetoPantallaVideo.SetActive(true);
            videoPlayerVictoria.Play();
        }
    }

    private void ActivarVideoDerrota()
    {
        if (musicaFondo != null) musicaFondo.Stop();

        // === PAUSAR EL JUEGO ===
        Time.timeScale = 0f;

        Debug.Log("GameManager: Ambos jugadores eliminados. Juego pausado y reproduciendo derrota...");
        if (objetoPantallaVideoDerrota != null && videoPlayerDerrota != null)
        {
            objetoPantallaVideoDerrota.SetActive(true);
            videoPlayerDerrota.Play();
        }
    }

    private void AlTerminarVideoVictoria(VideoPlayer vp)
    {
        // === REANUDAR EL TIEMPO ANTES DE CAMBIAR DE ESCENA ===
        Time.timeScale = 1f;
        SceneManager.LoadScene(nombreSiguienteEscena);
    }

    private void AlTerminarVideoDerrota(VideoPlayer vp)
    {
        Debug.Log("GameManager: Terminó el video de derrota. Cargando escena de Game Over...");
        // === REANUDAR EL TIEMPO ANTES DE CAMBIAR DE ESCENA ===
        Time.timeScale = 1f;
        SceneManager.LoadScene(nombreEscenaGameOver);
    }

    void OnDestroy()
    {
        if (videoPlayerVictoria != null) videoPlayerVictoria.loopPointReached -= AlTerminarVideoVictoria;
        if (videoPlayerDerrota != null) videoPlayerDerrota.loopPointReached -= AlTerminarVideoDerrota;
    }
}