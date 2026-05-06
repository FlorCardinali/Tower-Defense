using UnityEngine;

public class BeatemupCamera : MonoBehaviour
{
    public Transform player1, player2;

    // Asigna tus objetos de pared aquí en el Inspector
    public Transform paredIzquierda, paredDerecha;

    // Configura las distancias fijas
    public float alturaFijaY = 5f;
    public float distanciaFijaZ = -10f;
    public float distanciaParedAlCentro = 8f;

    public float smoothing = 15f;

    void LateUpdate()
    {
        if (player1 == null || player2 == null || paredIzquierda == null || paredDerecha == null) return;

        // 1. Buscamos el punto medio de los jugadores (en X)
        float medioX = (player1.position.x + player2.position.x) / 2f;

        // 2. Creamos la nueva posición de la cámara (como antes)
        Vector3 targetPosition = new Vector3(medioX, alturaFijaY, distanciaFijaZ);

        // 3. Movimiento suave de la cámara
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing * Time.deltaTime);
        float paredY = paredIzquierda.position.y; // O una fija
        float paredZ = 0f; // Donde caminan los jugadores

        paredIzquierda.position = new Vector3(transform.position.x - distanciaParedAlCentro, paredY, paredZ);
        paredDerecha.position = new Vector3(transform.position.x + distanciaParedAlCentro, paredY, paredZ);
    }
}