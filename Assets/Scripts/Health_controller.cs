using UnityEngine;
using System; // ¡Necesario para el Action!

public class Health_controller : MonoBehaviour, IDaniable
{
    public int vidaMaxima = 100;
    public int vidaActual;
    public GameObject efectoExplosionPrefab;

    // ESTO ES LO QUE TU BARRA BUSCA PARA FUNCIONAR
    public event Action<int, int> OnVidaCambiada;

    private bool estaSiendoGolpeado = false;

    void Start()
    {
        vidaActual = vidaMaxima;
        // Avisamos a la barra la vida inicial al arrancar
        OnVidaCambiada?.Invoke(vidaActual, vidaMaxima);
    }

    public void tomarDanio(int danio)
    {
        if (estaSiendoGolpeado) return;

        estaSiendoGolpeado = true;
        
        vidaActual -= danio;
        
        // Avisamos a la barra que la vida cambió
        OnVidaCambiada?.Invoke(vidaActual, vidaMaxima);

        if (vidaActual <= 0)
        {
            vidaActual = 0;
            Morir();
        }
        else
        {
            Invoke("ResetearGolpe", 0.3f);
        }
    }

    private void ResetearGolpe() 
    { 
        estaSiendoGolpeado = false; 
    }

    private void Morir()
    {
        if (efectoExplosionPrefab != null)
        {
            Instantiate(efectoExplosionPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}