using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public Transform camara;
    [Range(0f, 1f)] public float efectoParallax; // 0 significa estático en el mundo, 1 se mueve fijo con la cámara
    
    private float posicionInicialX;

    void Start()
    {
        posicionInicialX = transform.position.x;
        
        // Si no asignás la cámara en el inspector, el script la busca sola
        if (camara == null)
        {
            camara = Camera.main.transform;
        }
    }

    void Update()
    {
        // Calculamos cuánto debe acompañar este fondo al movimiento de la cámara
        float distanciaACompañar = camara.position.x * efectoParallax;
        
        // Actualizamos la posición del objeto solo en el eje X
        transform.position = new Vector3(posicionInicialX + distanciaACompañar, transform.position.y, transform.position.z);
    }
}