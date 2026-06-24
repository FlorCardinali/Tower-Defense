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
    [SerializeField] private GameObject objetoPantallaVideoDerrota; // El nuevo objeto de UI
    [SerializeField] private VideoPlayer videoPlayerDerrota;       // El nuevo reproductor
    [SerializeField] private string nombreEscenaGameOver = "PantallaPerdiste"; // Tu escena de restart

    public event Action OnBossDerrotado;
    private bool secuenciaFinalIniciada = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Escuchamos el final del video de victoria
        if (videoPlayerVictoria != null)
            videoPlayerVictoria.loopPointReached += AlTerminarVideoVictoria;

        // Escuchamos el final del video de derrota
        if (videoPlayerDerrota != null)
            videoPlayerDerrota.loopPointReached += AlTerminarVideoDerrota;
    }

    void Update()
    {
        // Si ya saltó un video (ganaste o perdiste), no hacemos nada más
        if (secuenciaFinalIniciada) return;

        // COMPROBACIÓN DE DERROTA: Si ambos jugadores fueron eliminados
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

        Debug.Log("GameManager: ¡Señal de victoria! Reproduciendo video...");
        if (objetoPantallaVideo != null && videoPlayerVictoria != null)
        {
            objetoPantallaVideo.SetActive(true);
            videoPlayerVictoria.Play();
        }
    }

    private void ActivarVideoDerrota()
    {
        Debug.Log("GameManager: Ambos jugadores eliminados. Reproduciendo video de derrota...");
        if (objetoPantallaVideoDerrota != null && videoPlayerDerrota != null)
        {
            objetoPantallaVideoDerrota.SetActive(true); // Mostramos el panel de derrota
            videoPlayerDerrota.Play();                  // Mandamos Play al video de perdiste
        }
    }

    private void AlTerminarVideoVictoria(VideoPlayer vp)
    {
        SceneManager.LoadScene(nombreSiguienteEscena);
    }

    private void AlTerminarVideoDerrota(VideoPlayer vp)
    {
        Debug.Log("GameManager: Terminó el video de derrota. Cargando escena de Game Over...");
        SceneManager.LoadScene(nombreEscenaGameOver); // Te manda a la escena donde está el botón reiniciar
    }

    void OnDestroy()
    {
        if (videoPlayerVictoria != null) videoPlayerVictoria.loopPointReached -= AlTerminarVideoVictoria;
        if (videoPlayerDerrota != null) videoPlayerDerrota.loopPointReached -= AlTerminarVideoDerrota;
    }
}
