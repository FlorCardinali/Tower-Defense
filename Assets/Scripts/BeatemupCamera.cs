using UnityEngine;

public class BeatemupCamera : MonoBehaviour
{
    public Transform player1, player2;

   
    public Transform paredIzquierda, paredDerecha;

    // Configura las distancias fijas
    public float alturaFijaY = 5f;
    public float distanciaFijaZ = -10f;
    public float distanciaParedAlCentro = 8f;

    public float smoothing = 15f;

    void LateUpdate()
    {
        // Las paredes son obligatorias, pero los jugadores ahora pueden faltar
        if (paredIzquierda == null || paredDerecha == null) return;

        // Si los dos murieron, no nos movemos más
        if (player1 == null && player2 == null) return;

        float medioX = 0f;

        // CASO 1: Los dos jugadores están vivos (Lógica clásica)
        if (player1 != null && player2 != null)
        {
            medioX = (player1.position.x + player2.position.x) / 2f;
        }
        // CASO 2: Solo queda vivo el Player 1
        else if (player1 != null)
        {
            medioX = player1.position.x;
        }
        // CASO 3: Solo queda vivo el Player 2
        else if (player2 != null)
        {
            medioX = player2.position.x;
        }

        // Creamos la nueva posición de la cámara basada en el objetivo válido
        Vector3 targetPosition = new Vector3(medioX, alturaFijaY, distanciaFijaZ);

        // Movimiento suave de la cámara
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing * Time.deltaTime);

        float paredY = paredIzquierda.position.y;
        float paredZ = 0f;

        // Las paredes se siguen moviendo con la cámara pase lo que pase
        paredIzquierda.position = new Vector3(transform.position.x - distanciaParedAlCentro, paredY, paredZ);
        paredDerecha.position = new Vector3(transform.position.x + distanciaParedAlCentro, paredY, paredZ);
    }
}