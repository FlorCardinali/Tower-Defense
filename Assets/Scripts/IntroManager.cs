using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class IntroManager : MonoBehaviour
{
    [Header("Referencias de Paneles")]
    public GameObject intro_Panel;
    public GameObject main_Menu_Panel;

    [Header("Configuración de Sonido")]
    public AudioSource audioSource;
    public AudioClip startSound;

    private bool introFinalizada = false;

    void Start()
    {
        intro_Panel.SetActive(true);
        main_Menu_Panel.SetActive(false);
    }

    void Update()
    {
        if (introFinalizada) return;

        // Detección de entrada (Teclado, Mouse, Mando)
        bool inputValido = (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame) ||
                           (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) ||
                           (Gamepad.current != null && Gamepad.current.allControls.Any(c => c is UnityEngine.InputSystem.Controls.ButtonControl b && b.wasPressedThisFrame));

        if (inputValido)
        {
            ComenzarJuego();
        }
    }

    void ComenzarJuego()
    {
        introFinalizada = true;

        if (audioSource != null && startSound != null)
        {
            audioSource.PlayOneShot(startSound);

            // Esto libera memoria sin cortar el audio
            Destroy(gameObject, startSound.length);
        }

        intro_Panel.SetActive(false);
        main_Menu_Panel.SetActive(true);

        Debug.Log("Intro finalizada. Objeto programado para destruirse.");
    }
}