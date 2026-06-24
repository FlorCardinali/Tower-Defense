using UnityEngine;
using System; 

public class Health_controller : MonoBehaviour, IDaniable
{
    public int vidaMaxima = 100;
    public int vidaActual;
    public GameObject efectoExplosionPrefab;


    public event Action<int, int> OnVidaCambiada;

    private bool estaSiendoGolpeado = false;

    void Start()
    {
        vidaActual = vidaMaxima;
        
        OnVidaCambiada?.Invoke(vidaActual, vidaMaxima);
    }

    public void tomarDanio(int danio)
    {
        if (estaSiendoGolpeado) return;

        estaSiendoGolpeado = true;
        
        vidaActual -= danio;
        
    
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