using UnityEngine;

public class Health_controller : MonoBehaviour, IDaniable 
{
    public int vidaMaxima = 40;
    public int vidaActual;

    void Start()
    {
        vidaActual = vidaMaxima;
    }

    public void tomarDanio(int danio)
    {
        if (vidaActual - danio <= 0)
        {
            vidaActual = 0;
            Morir();
        }
        else
        {

            vidaActual -= danio;
        }
    }

    private void Morir()
    {
        Debug.Log("Se murio");
        Destroy(gameObject);
    }
}
