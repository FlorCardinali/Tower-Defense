using UnityEngine;
using UnityEngine.UI;

public class BarraDeVidaUI : MonoBehaviour
{
    
    [SerializeField] private Health_controller controladorVidaPlayer;

    private Slider slider;

    void Awake()
    {
        slider = GetComponent<Slider>();
    }

    void OnEnable()
    {
        if (controladorVidaPlayer != null)
        {
            // La barra se "suscribe" al evento de ese player específico
            controladorVidaPlayer.OnVidaCambiada += ActualizarBarra;
        }
    }

    void OnDisable()
    {
        if (controladorVidaPlayer != null)
        {
            // Nos desuscribimos al destruirse o apagarse para evitar errores
            controladorVidaPlayer.OnVidaCambiada -= ActualizarBarra;
        }
    }

    private void ActualizarBarra(int vidaActual, int vidaMaxima)
    {
        slider.maxValue = vidaMaxima;
        slider.value = vidaActual;
    }
}