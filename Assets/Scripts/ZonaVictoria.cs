using UnityEngine;

public class ZonaVictoria : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Si el objeto que entró tiene el Tag de Player...
        if (other.CompareTag("Player"))
        {
            // Le mandamos la señal directa al GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ActivarVideoVictoria();
            }
        }
    }
}
