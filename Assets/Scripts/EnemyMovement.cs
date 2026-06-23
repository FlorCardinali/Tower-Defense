using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MovimientoEnemigoTorpe : MonoBehaviour
{
    [Header("Movimiento y Rotación")]
    [SerializeField] private float velocidadMovimiento = 2f;
    [SerializeField] private float velocidadRotacion = 2f;
    [SerializeField] private float rangoAtaque = 1.5f;

    [Header("Configuración de Ataque")]
    [SerializeField] private int danioAtaque = 10;
    [SerializeField] private float cadenciaAtaque = 2f;
    [SerializeField] private float retrasoDelGolpe = 0.5f;
    [SerializeField] private float descansoAtaque = 0.3f;
    private float proximoTiempoDeAtaque = 0f;
    private bool estaAtacando = false;

    [Header("Ángulo de Caminata (Torpeza)")]
    [Range(-1f, 1f)]
    [SerializeField] private float precisionDeFrente = 0.8f;

    [Header("Configuración de Objetivos")]
    [SerializeField] private float radioBusqueda = 15f;
    [SerializeField] private float margenHisteresis = 3.0f;

    private Transform jugadorObjetivo;
    private Transform[] todosLosJugadores;
    private NavMeshAgent agente;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        agente.updateRotation = false;

        GameObject[] objetosJugadores = GameObject.FindGameObjectsWithTag("Player");
        todosLosJugadores = new Transform[objetosJugadores.Length];

        for (int i = 0; i < objetosJugadores.Length; i++)
        {
            todosLosJugadores[i] = objetosJugadores[i].transform;
        }
    }

    void Update()
    {
        SeleccionarObjetivo();

        if (jugadorObjetivo != null)
        {
            MoverYRotarHaciaObjetivo();
        }
    }

    private void SeleccionarObjetivo()
    {
        if (todosLosJugadores == null || todosLosJugadores.Length == 0) return;

        Transform jugadorMasCercano = null;
        float distanciaMinima = float.MaxValue;

        foreach (Transform jugador in todosLosJugadores)
        {
            if (jugador == null) continue; // Evita al fantasma

            float distancia = Vector3.Distance(transform.position, jugador.position);
            if (distancia < distanciaMinima)
            {
                distanciaMinima = distancia;
                jugadorMasCercano = jugador;
            }
        }

        // 1. Si no hay ningún jugador vivo en la escena, nos limpiamos y frenamos
        if (jugadorMasCercano == null)
        {
            jugadorObjetivo = null;
            if (agente.isOnNavMesh) agente.ResetPath();
            return;
        }

        //  Si el más cercano está FUERA del radio de búsqueda, 
        // el enemigo pierde el objetivo y NO lo persigue de una.
        if (distanciaMinima > radioBusqueda)
        {
            jugadorObjetivo = null;
            if (agente.isOnNavMesh) agente.ResetPath();
            return;
        }

        //  Si sobrevivió al filtro de distancia, recién ahí lo fijamos como objetivo
        if (jugadorObjetivo == null)
        {
            jugadorObjetivo = jugadorMasCercano;
        }
        else
        {
            float distanciaObjetivoActual = Vector3.Distance(transform.position, jugadorObjetivo.position);
            if (distanciaMinima < distanciaObjetivoActual - margenHisteresis)
            {
                jugadorObjetivo = jugadorMasCercano;
            }
        }
    }

    private void MoverYRotarHaciaObjetivo()
    {
        if (estaAtacando) return;

        Vector3 direccion = jugadorObjetivo.position - transform.position;
        direccion.y = 0;

        float distancia = direccion.magnitude;
        Vector3 direccionNormalizada = direccion.normalized;
        float alineacion = Vector3.Dot(transform.forward, direccionNormalizada);

        if (direccion != Vector3.zero)
        {
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, velocidadRotacion * Time.deltaTime);
        }

        if (distancia > rangoAtaque)
        {
            if (alineacion >= precisionDeFrente)
            {
                agente.speed = velocidadMovimiento;
                agente.SetDestination(jugadorObjetivo.position);
            }
            else
            {
                agente.ResetPath();
            }
        }
        else
        {
            agente.ResetPath();
            if (Time.time >= proximoTiempoDeAtaque && alineacion >= precisionDeFrente)
            {
                StartCoroutine(SecuenciaDeAtaque());
                proximoTiempoDeAtaque = Time.time + cadenciaAtaque;
            }
        }
    }

    private IEnumerator SecuenciaDeAtaque()
    {
        estaAtacando = true;
        agente.ResetPath();

        Debug.Log("Casteando...");
        yield return new WaitForSeconds(retrasoDelGolpe);

        if (jugadorObjetivo != null)
        {
            Vector3 dirFinal = (jugadorObjetivo.position - transform.position);
            dirFinal.y = 0;

            // Comprobamos si sigue en rango y si sigue enfrente de su cara
            if (dirFinal.magnitude <= rangoAtaque && Vector3.Dot(transform.forward, dirFinal.normalized) >= precisionDeFrente)
            {
                Debug.Log($"Golpie a: {jugadorObjetivo.name}");

                // ==========================================
                // ACA VA LA LOGICA DE DAÑO CON TU INTERFAZ
                // ==========================================
                IDaniable objetivo = jugadorObjetivo.GetComponent<IDaniable>();
                if (objetivo != null)
                {
                    objetivo.tomarDanio(danioAtaque); // Llamamos a tu interfaz
                }
                // ==========================================
            }
            else
            {
                Debug.Log("Fallo en ataque");
            }
        }
        yield return new WaitForSeconds(descansoAtaque);
        estaAtacando = false;
    }
}