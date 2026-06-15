using UnityEngine;
using System; 

public class Health_controller : MonoBehaviour, IDaniable
{
    public int vidaMaxima = 40;
    public int vidaActual;

    //  Este evento avisará a la UI cada vez que la vida cambie (pasa vidaActual y vidaMaxima)
    public event Action<int, int> OnVidaCambiada;

    void Start()
    {
        vidaActual = vidaMaxima;

        // Avisamos la vida inicial al arrancar el juego para que la UI se llene
        OnVidaCambiada?.Invoke(vidaActual, vidaMaxima);
    }

    public void tomarDanio(int danio)
    {
        if (vidaActual - danio <= 0)
        {
            vidaActual = 0;
            // Avisamos que la vida llegó a 0 antes de destruir el objeto
            OnVidaCambiada?.Invoke(vidaActual, vidaMaxima);
            Morir();
        }
        else
        {
            vidaActual -= danio;
            // Avisamos que la vida bajó
            OnVidaCambiada?.Invoke(vidaActual, vidaMaxima);
        }
    }

    private void Morir()
    {
        Debug.Log(gameObject.name + " se murió.");
        Destroy(gameObject);
    }
}
