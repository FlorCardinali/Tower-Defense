using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class IA_BossCubo : MonoBehaviour
{
    private enum EstadoBoss { Buscando, PreparandoDash, EjecutandoDash, Aturdido }
    [SerializeField] private EstadoBoss estadoActual = EstadoBoss.Buscando;

    [Header("Referencias")]
    private Transform jugadorObjetivo;
    private NavMeshAgent agente;
    private Health_controller vida;

    [Header("Configuración de Persecución")]
    [SerializeField] private float radioBusqueda = 12f;
    [SerializeField] private float velocidadNormal = 3.5f;

    [Header("Mecánica de Dash y Ataque")]
    [SerializeField] private float distanciaParaDash = 6f;
    [SerializeField] private float tiempoPreparacion = 0.8f;
    [SerializeField] private float velocidadDash = 18f;
    [SerializeField] private float duracionMaximaDash = 0.5f;
    [SerializeField] private int danioDash = 25;

    [Header("Mecánica de Aturdimiento (Stun)")]
    [SerializeField] private float tiempoAturdido = 3f;
    [SerializeField] private Color colorNormal = Color.red;
    [SerializeField] private Color colorAturdido = Color.yellow;
    private Renderer miRenderer;

    private Vector3 direccionDash;
    private float cronometroDash;

    void Awake()
    {
        agente = GetComponent<NavMeshAgent>();
        vida = GetComponent<Health_controller>();
        miRenderer = GetComponent<Renderer>();
        if (miRenderer != null) miRenderer.material.color = colorNormal;
    }

    void Update()
    {
        // Si el Boss no tiene NavMeshAgent activo por alguna razón, salimos
        if (!agente.isOnNavMesh) return;

        switch (estadoActual)
        {
            case EstadoBoss.Buscando:
                BuscarYPerseguir();
                break;

            case EstadoBoss.PreparandoDash:
                // Se queda quieto mirando al jugador mientras carga el ataque
                FrenarYMirarObjetivo();
                break;

            case EstadoBoss.EjecutandoDash:
                ActualizarDash();
                break;

            case EstadoBoss.Aturdido:
                // No hace nada, el NavMeshAgent está apagado temporalmente
                break;
        }
    }

    private void BuscarYPerseguir()
    {
        agente.speed = velocidadNormal;

        // Buscamos al jugador más cercano usando la etiqueta que ya tenías
        GameObject[] jugadores = GameObject.FindGameObjectsWithTag("Player");
        float distanciaMinima = float.MaxValue;
        Transform masCercano = null;

        foreach (GameObject jug in jugadores)
        {
            if (jug == null) continue;
            float dist = Vector3.Distance(transform.position, jug.transform.position);
            if (dist < distanciaMinima)
            {
                distanciaMinima = dist;
                masCercano = jug.transform;
            }
        }

        jugadorObjetivo = masCercano;

        if (jugadorObjetivo != null && distanciaMinima <= radioBusqueda)
        {
            agente.SetDestination(jugadorObjetivo.position);

            // SI SE ACERCA LO SUFICIENTE, ACTIVA EL DASH
            if (distanciaMinima <= distanciaParaDash)
            {
                StartCoroutine(SecuenciaPrepararDash());
            }
        }
        else
        {
            agente.ResetPath(); // Se queda quieto si no hay nadie en rango
        }
    }

    private IEnumerator SecuenciaPrepararDash()
    {
        estadoActual = EstadoBoss.PreparandoDash;

        // === MEJORA: Frenamos y desactivamos el agente de inmediato para que no pelee con las físicas ===
        agente.ResetPath();
        agente.enabled = false;

        // Si tenés un Rigidbody, anulamos cualquier velocidad o empuje previo del jugador
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero; // En versiones anteriores de Unity usa rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Pequeño salto visual/anticipación (sube un poquito en Y)
        transform.position += Vector3.up * 0.4f;

        // Se queda congelado apuntando al jugador los segundos de preparación
        float tiempoPasado = 0f;
        while (tiempoPasado < tiempoPreparacion)
        {
            FrenarYMirarObjetivo();
            tiempoPasado += Time.deltaTime;
            yield return null; // Espera al siguiente frame
        }

        if (jugadorObjetivo != null)
        {
            // Calculamos la dirección fija hacia donde estaba el player
            direccionDash = (jugadorObjetivo.position - transform.position);
            direccionDash.y = 0;
            direccionDash.Normalize();

            estadoActual = EstadoBoss.EjecutandoDash;
            cronometroDash = 0f;
        }
        else
        {
            // Si el jugador desapareció, volvemos a patrullar
            estadoActual = EstadoBoss.Buscando;
            agente.enabled = true;
        }
    }

    private void ActualizarDash()
    {
        cronometroDash += Time.deltaTime;

        // Movemos al Boss de forma súper rápida en la dirección calculada
        transform.Translate(direccionDash * velocidadDash * Time.deltaTime, Space.World);

        // Si se acaba el tiempo del Dash, significa que falló y queda aturdido
        if (cronometroDash >= duracionMaximaDash)
        {
            StartCoroutine(SecuenciaAturdimiento());
        }
    }

    private IEnumerator SecuenciaAturdimiento()
    {
        estadoActual = EstadoBoss.Aturdido;
        Debug.Log("¡El Boss falló el Dash y quedó ATURDIDO!");

        // Se pone amarillo
        if (miRenderer != null) miRenderer.material.color = colorAturdido;

        // Anulamos cualquier fuerza del choque para que se quede quieto
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.linearVelocity = Vector3.zero;

        yield return new WaitForSeconds(tiempoAturdido);

        // Se recupera
        if (miRenderer != null) miRenderer.material.color = colorNormal;

        // === RE-ACTIVACIÓN CLAVE ===
        agente.enabled = true;
        estadoActual = EstadoBoss.Buscando;
    }

    private void FrenarYMirarObjetivo()
    {
        if (jugadorObjetivo == null) return;
        Vector3 dirMirar = jugadorObjetivo.position - transform.position;
        dirMirar.y = 0;
        if (dirMirar != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(dirMirar);
        }
    }

    // DETECCIÓN DE IMPACTO DURANTE EL DASH
    private void OnCollisionEnter(Collision collision)
    {
        if (estadoActual == EstadoBoss.EjecutandoDash)
        {
            // Intentamos ver si chocamos con un Player que implemente IDaniable
            IDaniable objetivo = collision.gameObject.GetComponent<IDaniable>();
            if (objetivo != null)
            {
                Debug.Log("¡El Boss embistió a un jugador!");
                objetivo.tomarDanio(danioDash);

                // Al pegar con éxito, no queda aturdido tanto tiempo o vuelve a buscar directo
                StartCoroutine(SecuenciaAturdimiento());
            }
        }
    }
}
