using UnityEngine;

public class BeatemupCamera : MonoBehaviour
{
    public Transform player1, player2;
    
    // Esta es la posición fija en la que querés que se quede la cámara.
    // Solo ajustá estos números en el Inspector para alejarla o acercarla.
    public float alturaFijaY = 5f;
    public float distanciaFijaZ = -10f; 
    
    public float smoothing = 5f;

    void LateUpdate()
    {
        if (player1 == null || player2 == null) return;

        // 1. Buscamos el punto medio de los jugadores (pero SOLO en el eje X)
        float medioX = (player1.position.x + player2.position.x) / 2f;

        // 2. Creamos la nueva posición de la cámara:
        // Toma la X de los jugadores, pero mantiene SU PROPIA altura (Y) y distancia (Z) fijas.
        Vector3 targetPosition = new Vector3(medioX, alturaFijaY, distanciaFijaZ);

        // 3. Movimiento suave
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing * Time.deltaTime);
    }
}