using UnityEngine;

public class PuertaBoss : MonoBehaviour
{
    [Header("Configuración del Movimiento")]
    [SerializeField] private Vector3 direccionDesplazamiento = new Vector3(0, 0, 5f); // Se moverá 5 unidades en Z
    [SerializeField] private float velocidadSeccion = 2f;

    private Vector3 posicionObjetivo;
    private bool debeAbrirse = false;

    void Start()
    {
        // Calculamos dónde debería terminar la puerta al abrirse
        posicionObjetivo = transform.position + direccionDesplazamiento;

        // SEGURO EXTRA: Nos suscribimos en el Start para dar tiempo a que el GameManager se cree en su Awake
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnBossDerrotado += IniciarApertura;
            Debug.Log($"Puerta [{gameObject.name}]: ¡Suscrita con éxito al evento del Boss!");
        }
        else
        {
            Debug.LogError($"Puerta [{gameObject.name}]: ¡ERROR! No se encontró el GameManager en la escena.");
        }
    }

    void OnDisable()
    {
        // Nos desuscribimos para evitar errores de memoria al cerrar el juego o cambiar de escena
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnBossDerrotado -= IniciarApertura;
        }
    }

    // Este método se ejecuta en el milisegundo en que el Boss muere
    private void IniciarApertura()
    {
        Debug.Log("Puerta: Recibí la señal del Boss. ¡Iniciando desplazamiento!");
        debeAbrirse = true;
    }

    void Update()
    {
        // Si el Boss ya murió, movemos la puerta suavemente hacia la posición objetivo
        if (debeAbrirse)
        {
            transform.position = Vector3.MoveTowards(transform.position, posicionObjetivo, velocidadSeccion * Time.deltaTime);

            // Si ya llegó a su destino, dejamos de moverla
            if (transform.position == posicionObjetivo)
            {
                debeAbrirse = false;
                Debug.Log("Puerta abierta por completo.");
            }
        }
    }
}